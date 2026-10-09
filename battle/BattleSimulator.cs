#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattleSimulator : Node
{
    private const float messageTime = 1.0f;
    private const string hitMissedMessage = "But it missed..!";

    [Export]
    public bool autorun = false;

    [ExportCategory("Components")]
    [Export]
    public Godot.Collections.Array<BattlerController> controllers = [];

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
    private Godot.Collections.Array<HPBox> HpBoxes { get; set; } = [];

    [Export]
    private Godot.Collections.Array<BattlerSprite> Sprites { get; set; } = [];

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
        RegisterControllers();

        Battle.TurnCompleted += OnTurnCompleted;
        
        await PlayIntro();

        if (autorun)
            await RunSimulation();
    }

    public override void _ExitTree()
    {
        Battle.TurnCompleted -= OnTurnCompleted;
    }

    public void OnTurnCompleted()
    {
        TurnCount++;
    }

    private async Task PlayIntro()
    {
        var introAnim = new StringName("intro");
        var resetAnim = new StringName("RESET");
        
        BattleBox?.ShowMessage($"{Battle.Battlers[1].Name} appeared!"); // TODO: fragile

        if (AnimationPlayer is null) return;

        AnimationPlayer.Play(introAnim);
        await ToSignal(AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        AnimationPlayer.Play(resetAnim);
    }

    private void RegisterControllers()
    {
        foreach (var c in controllers)
        {
            // create references to quickly look up sprites battlers/sprites/hpboxes by controller
            var r = new BattlerReference
            {
                Controller = c,
                Battler = c.Battler,
                Sprite = Sprites.ElementAtOrDefault(controllers.IndexOf(c)), // TODO: dont rely on array order? maybe tie these objects together more explicitly
                HpBox = HpBoxes.ElementAtOrDefault(controllers.IndexOf(c)),
            };

            References.Add(r);

            // register a player
            if (c is PlayerBattlerController || c == controllers[0])
            {
                Player = c;
            }

            if (!Battle.Battlers.Contains(c.Battler))
            {
                Battle.Battlers.Add(c.Battler);
                GD.Print($"Registered battler {c.Battler.Name}!");
            }
        }
    }

    public bool IsPlayerTurn() => Battle.ActiveBattler == Player.Battler;

    public async Task Step()
    {
        Battle.ProcessState();
        UpdateBattleBox();

        var controller = GetControllerFor(Battle.ActiveBattler);

        switch (Battle.Phase)
        {
            case BattleState.BattlePhase.Start:
                break;
            case BattleState.BattlePhase.Choice:
                await StepChoice(controller);
                break;
            case BattleState.BattlePhase.Move:
                await StepMove(controller);
                break;
            case BattleState.BattlePhase.PostMove:
                await StepPostMove(controller);
                break;
            case BattleState.BattlePhase.End:
                await StepEnd();
                break;
            default:
                break;
        }
    }

    private void UpdateBattleBox()
    {
        if (BattleBox is null) return;

        BattleBox.Update(Battle.Phase);
        var message = BattleBox.GetMessage(Battle);

        if (message is null) return;

        GD.Print($"{TurnCount + 1}. {message}");
        BattleBox.ShowMessage(message);
    }

    private async Task StepEnd()
    {
        await ShowMessage("Battle over.");
        var livingBattlers = Battle.GetLivingBattlers();

        if (livingBattlers.Count == 0)
            await ShowMessage("It was a draw!");
        else if (livingBattlers.Contains(Player.Battler))
            await ShowMessage("You win!");
        else
            await ShowMessage("You lost...");
    }

    private async Task StepPostMove(BattlerController? controller)
    {
        if (controller is null)
            return;
        foreach (var e in controller.StatusEffects.Duplicate())
        {
            await PresentAction(new PlannedAction(e, controller, controller)); // TODO: what??
        }
    }

    private async Task StepMove(BattlerController? controller)
    {
        var sprite = GetSpriteFor(Battle.ActiveBattler);
        if (controller is null)
            return;
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

        if (sprite is not null)
        {
            await sprite.PlayAnimation(controller.CurrentMove.GetAnimation()); // TODO: missed move
        }

        foreach (var action in ResolveMove(controller))
        {
            if (action.Target.Battler.IsFainted() && action.Target != action.User)
                break;
            await PresentAction(action);
        }
    }

    private static async Task StepChoice(BattlerController? controller)
    {
        controller?.CurrentMove = await controller.ChooseMove();
    }

    public async Task ShowMessage(string text, float duration = messageTime)
    {
        if (BattleBox is null)
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
        }
    }

    public BattlerReference? GetReferenceFor(Battler? battler) =>
        battler is null ? null : References.FirstOrDefault(r => r.Battler == battler);

    public BattlerController? GetControllerFor(Battler? battler) =>
        GetReferenceFor(battler)?.Controller;

    private BattlerSprite? GetSpriteFor(Battler? battler) => GetReferenceFor(battler)?.Sprite;

    private HPBox? GetHpBoxFor(Battler? battler) => GetReferenceFor(battler)?.HpBox;

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

            var spr = GetSpriteFor(action.Target.Battler);
            if (spr is not null && action.Result.HasAnimation())
            {
                await spr.PlayAnimation(action.Result.GetAnimation());
            }

            RefreshHpboxes();

            var hpBox = GetHpBoxFor(action.Target.Battler);
            if (hpBox is not null && !hpBox.IsStable())
            {
                await ToSignal(hpBox, HPBox.SignalName.HpChangeFinished);
            }

            var hitMessage = action.Result.GetHitMessage();
            if (hitMessage is not null)
                await ShowMessage(hitMessage);
        }

        var resultMessage = action.Result.GetResultMessage();
        if (resultMessage is null)
            return;
        await ShowMessage(hits > 0 ? resultMessage : hitMissedMessage);
    }
}
