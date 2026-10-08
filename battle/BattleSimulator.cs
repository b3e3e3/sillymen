#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Sillymen;

public record BattlerReference
{
    public required Battler battler { get; init; }
    public required BattlerController controller { get; init; }
    public BattlerSprite? sprite;
    public HPBox? hp_box;
}

[GlobalClass]
public partial class BattleSimulator : Node
{
    private const int MAX_STATES = 100;
    private const float MSG_TIME = 1.0f;
    private const string HIT_MISSED_MSG = "But it missed..!";

    [Export] public bool autorun = false;

    [ExportCategory("Components")]
    [Export] public Array<BattlerController> controllers = [];
    [Export] public required BattleState battle { get; set; }
    [Export] public TypeChart? type_chart;

    // TODO: Decouple UI
    [ExportCategory("UI")]
    [Export] public BattleBox? battle_box { get; set; }
    [Export] private AnimationPlayer? animation_player { get; set; }
    [Export] private Array<HPBox> hp_boxes = [];
    [Export] private Array<BattlerSprite> sprites = [];

    public required BattlerController player { get; set; }
    public int turn_count = 0;
    public bool simulating = false;

    private List<BattlerReference> references = [];

    public BattleSimulator() { }
    public BattleSimulator(BattleState state)
    {
        battle = state;
    }

    public async override void _Ready()
    {
        type_chart ??= GD.Load<TypeChart>("res://battle/battler/type/default_type_chart.tres");

        foreach (var c in controllers)
        {
            var r = new BattlerReference
            {
                controller = c,
                battler = c.battler,
                sprite = sprites.ElementAtOrDefault(controllers.IndexOf(c)),
                hp_box = hp_boxes.ElementAtOrDefault(controllers.IndexOf(c)),
            };

            references.Add(r);

            if (c.GetType() == typeof(PlayerBattlerController) || c == controllers[0])
            {
                player = c;
            }

            if (battle.battlers.Contains(c.battler) == false)
            {
                battle.battlers.Add(c.battler);
                GD.Print($"Registered battler {c.battler.name}!");
            }
        }

        battle.TurnCompleted += () => turn_count++;

        battle_box?.show_message($"{battle.battlers[1].name} appeared!");

        if (animation_player != null)
        {
            animation_player.Play("intro");
            await ToSignal(animation_player, AnimationPlayer.SignalName.AnimationFinished);
            animation_player.Play("RESET");

        }

        if (autorun)
        {
            await run_simulation();
        }
    }

    public bool is_player_turn() => battle.active_battler == player.battler;

    public async Task step()
    {
        // log_current_state();

        battle.process_state();

        GD.Print("We step");
        GD.Print(Enum.GetName(battle.phase));
        GD.Print(Enum.GetName(battle_box!.screen));

        if (battle_box != null)
        {
            battle_box.update(battle.phase);
            string? message = battle_box.get_message(battle);
            if (message != null)
            {
                GD.Print($"{turn_count + 1}. {message}");
                battle_box.show_message(message);
            }
        }

        var controller = get_controller_for(battle.active_battler);
        var sprite = get_sprite_for(battle.active_battler);

        switch (battle.phase)
        {
            case BattleState.Phase.CHOICE:
                controller?.current_move = await controller.choose_move();
                break;
            case BattleState.Phase.MOVE:
                if (controller == null) break;
                foreach (var e in controller.status_effects.Duplicate())
                {
                    if (e.has_expired())
                    {
                        controller.remove_status_effect(e);
                        await show_message($"{controller.battler.name} no longer has {e.name}!");
                        continue;
                    }
                }

                if (sprite != null)
                {
                    await sprite.play_animation(controller.current_move.get_animation()); // TODO: missed move
                }

                foreach (var action in resolve_move(controller))
                {
                    if (action.Target.battler.is_fainted() && action.Target != action.User)
                        break;
                    await present_action(action);
                }
                break;
            case BattleState.Phase.POST_MOVE:
                if (controller == null) break;
                foreach (var e in controller.status_effects.Duplicate())
                {
                    await present_action(new PlannedAction(e, controller, controller)); // TODO: what??
                }
                break;
            case BattleState.Phase.END:
                await show_message("Battle over.");
                var living_battlers = battle.get_living_battlers();

                if (living_battlers.Count == 0)
                    await show_message("It was a draw!");
                else if (living_battlers.Contains(player.battler))
                    await show_message("You win!");
                else
                    await show_message("You lost...");
                break;
        }
    }

    public void update_battle_box()
    {
        if (battle_box == null) return;

        var message = battle_box.get_message(battle);
        if (message == null) return;

        battle_box.show_message(message);
    }

    public async Task show_message(string text, float duration = MSG_TIME)
    {
        if (battle_box == null) return;

        battle_box.show_message(text);
        await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);
    }

    public async Task run_simulation()
    {
        try
        {
            simulating = true;
            while (simulating)
            {
                await step();
                simulating = battle.phase != BattleState.Phase.END;
            }
        }
        catch (Exception e)
        {
            GD.PushError(e.ToString());
            throw;
        }
    }

    public BattlerController? get_controller_for(Battler? battler) =>
    battler == null ? null
        : references.FirstOrDefault(r => r.battler == battler)?.controller;

    private BattlerSprite? get_sprite_for(Battler? battler) =>
    battler == null ? null
            : references.FirstOrDefault(r => r.battler == battler)?.sprite;

    // private HPBox? get_hpbox_for(BattlerController? controller) => (from r in references where r.controller == controller select r.hp_box).First();


    private void refresh_hpboxes()
    {
        if (hp_boxes.Count == 0) return;
        foreach (var r in references)
        {
            r.hp_box?.update();
        }
    }

    private List<PlannedAction> resolve_move(BattlerController controller, Move? move = null)
    {
        List<PlannedAction> actions = [];
        foreach (var result in (move ?? controller.current_move).results)
        {
            actions.AddRange(resolve_move_result(controller, result));
        }
        return actions;
    }

    private List<PlannedAction> resolve_move_result(BattlerController controller, MoveResult result)
    {
        List<PlannedAction> actions = [];
        var valid_targets = result.get_valid_targets(controller.battler, battle.battlers ?? []);

        if (valid_targets.Count > 0)
        {
            var target_controller = get_controller_for(valid_targets[0]);
            actions.Add(new PlannedAction(result, controller, target_controller));
        }

        return actions;
    }

    private async Task present_action(PlannedAction action)
    {
        var hits = 0;

        for (var i = 0; i < action.Result.get_repeat_count(); i++)
        {
            if (action.Target.battler.is_fainted()) break;
            if (!action.ApplyMoveResult()) break;

            hits++;
            refresh_hpboxes();

            var spr = get_sprite_for(action.Target.battler);
            if (spr != null && action.Result.has_animation())
            {
                await spr.play_animation(action.Result.get_animation());
            }

            var hit_msg = action.Result.get_hit_message();
            if (hit_msg != null) await show_message(hit_msg);
        }

        var result_msg = action.Result.get_result_message();
        if (result_msg == null) return;
        await show_message(hits > 0 ? result_msg : HIT_MISSED_MSG);
    }

    // private Dictionary<Battler, BattlerController> controller_by_battler;
    // private Dictionary<Battler, BattlerSprite> sprite_by_battler;
    // private Dictionary<BattlerController, HPBox> hpbox_by_controller;
}