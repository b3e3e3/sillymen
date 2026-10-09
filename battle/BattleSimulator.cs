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
    public required Battler Battler { get; init; }
    public required BattlerController Controller { get; init; }
    public BattlerSprite? Sprite { get; set; }
    public HPBox? HpBox { get; set; }
}

[GlobalClass]
public partial class BattleSimulator : Node
{
    // private const int maxStates = 100;
    private const float messageTime = 1.0f;
    private const string hitMissedMessage = "But it missed..!";

    [Export]
    public bool autorun = false;

    [ExportCategory("Components")]
    [Export]
    public Array<BattlerController> controllers = [];

    [Export]
    public required BattleState Battle { get; set; }

    [Export]
    public TypeChart? TypeChart { get; set; }

    // TODO: Decouple UI
    [ExportCategory("UI")]
    [Export]
    public BattleBox? BattleBox { get; set; }

    [Export]
    private AnimationPlayer? AnimationPlayer { get; set; }

    [Export]
    private Array<HPBox> HpBoxes { get; set; } = [];

    [Export]
    private Array<BattlerSprite> Sprites { get; set; } = [];

    public required BattlerController Player { get; set; }
    public int TurnCount { get; set; } = 0;
    public bool Simulating { get; set; } = false;

    private List<BattlerReference> References { get; set; } = [];

    public BattleSimulator() { }

    public BattleSimulator(BattleState state)
    {
        Battle = state;
    }

    public override async void _Ready()
    {
        TypeChart ??= GD.Load<TypeChart>("res://battle/battler/type/default_type_chart.tres");

        foreach (var c in controllers)
        {
            var r = new BattlerReference
            {
                Controller = c,
                Battler = c.Battler,
                Sprite = Sprites.ElementAtOrDefault(controllers.IndexOf(c)),
                HpBox = HpBoxes.ElementAtOrDefault(controllers.IndexOf(c)),
            };

            References.Add(r);

            if (c.GetType() == typeof(PlayerBattlerController) || c == controllers[0])
            {
                Player = c;
            }

            if (Battle.Battlers.Contains(c.Battler) == false)
            {
                Battle.Battlers.Add(c.Battler);
                GD.Print($"Registered battler {c.Battler.Name}!");
            }
        }

        Battle.TurnCompleted += () => TurnCount++;

        BattleBox?.ShowMessage($"{Battle.Battlers[1].Name} appeared!");

        if (AnimationPlayer != null)
        {
            AnimationPlayer.Play("intro");
            await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
            AnimationPlayer.Play("RESET");
        }

        if (autorun)
        {
            await RunSimulation();
        }
    }

    public bool IsPlayerTurn() => Battle.ActiveBattler == Player.Battler;

    public async Task Step()
    {
        // LogCurrentState();

        Battle.ProcessState();

        GD.Print("We step");
        GD.Print(Enum.GetName(Battle.Phase));
        GD.Print(Enum.GetName(BattleBox!.screen));

        if (BattleBox != null)
        {
            BattleBox.Update(Battle.Phase);
            string? message = BattleBox.GetMessage(Battle);
            if (message != null)
            {
                GD.Print($"{TurnCount + 1}. {message}");
                BattleBox.ShowMessage(message);
            }
        }

        var controller = GetControllerFor(Battle.ActiveBattler);
        var sprite = GetSpriteFor(Battle.ActiveBattler);

        switch (Battle.Phase)
        {
            default:
            case BattleState.BattlePhase.Start:
                break;
            case BattleState.BattlePhase.Choice:
                controller?.CurrentMove = await controller.ChooseMove();
                break;
            case BattleState.BattlePhase.Move:
                if (controller == null)
                    break;
                foreach (var e in controller.StatusEffects.Duplicate())
                {
                    GD.Print($"{e.Name} has expired? {e.HasExpired()} ({e.count}/{e.MaxTurns})");
                    if (e.HasExpired())
                    {
                        controller.RemoveStatusEffect(e);
                        await ShowMessage($"{controller.Battler.Name} no longer has {e.Name}!");
                        continue;
                    }
                }

                if (sprite != null)
                {
                    await sprite.PlayAnimation(controller.CurrentMove.GetAnimation()); // TODO: missed move
                }

                foreach (var action in ResolveMove(controller))
                {
                    if (action.Target.Battler.IsFainted() && action.Target != action.User)
                        break;
                    await PresentAction(action);
                }
                break;
            case BattleState.BattlePhase.PostMove:
                if (controller == null)
                    break;
                foreach (var e in controller.StatusEffects.Duplicate())
                {
                    await PresentAction(new PlannedAction(e, controller, controller)); // TODO: what??
                }
                break;
            case BattleState.BattlePhase.End:
                await ShowMessage("Battle over.");
                var livingBattlers = Battle.GetLivingBattlers();

                if (livingBattlers.Count == 0)
                    await ShowMessage("It was a draw!");
                else if (livingBattlers.Contains(Player.Battler))
                    await ShowMessage("You win!");
                else
                    await ShowMessage("You lost...");
                break;
        }
    }

    public void UpdateBattleBox()
    {
        if (BattleBox == null)
            return;

        var message = BattleBox.GetMessage(Battle);
        if (message == null)
            return;

        BattleBox.ShowMessage(message);
    }

    public async Task ShowMessage(string text, float duration = messageTime)
    {
        if (BattleBox == null)
            return;

        BattleBox.ShowMessage(text);
        await ToSignal(GetTree().CreateTimer(duration), SceneTreeTimer.SignalName.Timeout);
    }

    public async Task RunSimulation()
    {
        try
        {
            Simulating = true;
            while (Simulating)
            {
                await Step();
                Simulating = Battle.Phase != BattleState.BattlePhase.End;
            }
        }
        catch (Exception e)
        {
            GD.PushError(e.ToString());
            throw;
        }
    }

    public BattlerController? GetControllerFor(Battler? battler) =>
        battler == null ? null : References.FirstOrDefault(r => r.Battler == battler)?.Controller;

    private BattlerSprite? GetSpriteFor(Battler? battler) =>
        battler == null ? null : References.FirstOrDefault(r => r.Battler == battler)?.Sprite;

    private void RefreshHpboxes()
    {
        if (HpBoxes.Count == 0)
            return;
        foreach (var r in References)
        {
            r.HpBox?.Update();
        }
    }

    private List<PlannedAction> ResolveMove(BattlerController controller, Move? move = null)
    {
        List<PlannedAction> actions = [];
        foreach (var result in (move ?? controller.CurrentMove).Results)
        {
            actions.AddRange(ResolveMoveResult(controller, result));
        }
        return actions;
    }

    private List<PlannedAction> ResolveMoveResult(BattlerController controller, MoveResult result)
    {
        List<PlannedAction> actions = [];
        var validTargets = result.GetValidTargets(controller.Battler, Battle.Battlers ?? []);

        if (validTargets.Count > 0)
        {
            var targetController = GetControllerFor(validTargets[0]);
            actions.Add(new PlannedAction(result, controller, targetController));
        }

        return actions;
    }

    private async Task PresentAction(PlannedAction action)
    {
        var hits = 0;

        for (var i = 0; i < action.Result.GetRepeatCount(); i++)
        {
            if (action.Target.Battler.IsFainted())
                break;
            if (!action.ApplyMoveResult())
                break;

            hits++;
            RefreshHpboxes();

            var spr = GetSpriteFor(action.Target.Battler);
            if (spr != null && action.Result.HasAnimation())
            {
                await spr.PlayAnimation(action.Result.GetAnimation());
            }

            var hitMessage = action.Result.GetHitMessage();
            if (hitMessage != null)
                await ShowMessage(hitMessage);
        }

        var resultMessage = action.Result.GetResultMessage();
        if (resultMessage == null)
            return;
        await ShowMessage(hits > 0 ? resultMessage : hitMissedMessage);
    }
}
