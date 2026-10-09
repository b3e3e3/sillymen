#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattleBox : Control
{
    public enum ScreenType
    {
        Normal,
        MoveChoice,
    }

    [Signal]
    public delegate void MoveSelectedEventHandler(Move move);

    [Export]
    public BattleSimulator? Simulator { get; set; }

    [Export]
    private Control? DialogContainer { get; set; }

    [Export]
    private RichTextLabel? DialogLabel { get; set; }

    [Export]
    private Control? ChoiceContainer { get; set; }

    [Export]
    private Control? MovesContainer { get; set; }

    private string message = "";
    public ScreenType screen = ScreenType.Normal;

    private void BuildMoveChoices(Battler battler)
    {
        var children = MovesContainer?.GetChildren() ?? [];
        if (children.Count > 0)
            return; // TODO: if moves need to change during battle, dont do this

        var scene = GD.Load<PackedScene>("res://battle/ui/move_button.tscn");

        // foreach (var c in children)
        // {
        //     c.QueueFree();
        // }

        foreach (var move in battler.Moves)
        {
            var button = scene.Instantiate<Button>();
            button.Text = move.Name;
            button.Connect(
                Button.SignalName.Pressed,
                Callable.From(() =>
                {
                    OnMoveButtonPressed(move);
                })
            ); //, (uint)ConnectFlags.OneShot);
            Callable
                .From(() =>
                {
                    MovesContainer?.AddChild(button);
                })
                .CallDeferred();
        }
    }

    public void ShowMessage(string text)
    {
        message = text;
        if (Simulator is not null)
            Update(Simulator.Battle.Phase);
    }

    public string? GetMessage(BattleState state)
    {
        switch (state.Phase)
        {
            case BattleState.BattlePhase.Choice:
                if (Simulator?.IsPlayerTurn() == true)
                {
                    return $"What will {state.ActiveBattler?.Name} do?";
                }
                break;
            case BattleState.BattlePhase.Move:
                GD.Print($"{state.ActiveBattler?.Name} is about to use a move!");

                var controller = Simulator?.GetControllerFor(state.ActiveBattler);
                return $"{state.ActiveBattler?.Name} used {controller?.CurrentMove.Name}!";
            case BattleState.BattlePhase.Start:
                break;
            case BattleState.BattlePhase.PostMove:
                break;
            case BattleState.BattlePhase.End:
                break;
            default:
                break;
        }

        return null;
    }

    public void Update(BattleState.BattlePhase phase)
    {
        DialogLabel?.Text = message; // GetMessage(state);

        switch (phase)
        {
            case BattleState.BattlePhase.Choice:
                if (screen == ScreenType.Normal)
                {
                    DialogContainer?.Visible = true;
                    ChoiceContainer?.Visible = Simulator?.IsPlayerTurn() == true;
                    MovesContainer?.Visible = false;
                }
                else if (screen == ScreenType.MoveChoice)
                {
                    DialogContainer?.Visible = false;
                    ChoiceContainer?.Visible = false;
                    MovesContainer?.Visible = true;
                }
                break;
            case BattleState.BattlePhase.Move:
            case BattleState.BattlePhase.PostMove:
                DialogContainer?.Visible = true;
                ChoiceContainer?.Visible = false;
                MovesContainer?.Visible = false;
                break;
            case BattleState.BattlePhase.Start:
                break;
            case BattleState.BattlePhase.End:
                break;

            default:
                DialogContainer?.Visible = true;
                ChoiceContainer?.Visible = false;
                MovesContainer?.Visible = false;
                break;
        }
    }

    private void OnMoveButtonPressed(Move move)
    {
        screen = ScreenType.Normal;
        EmitSignal(SignalName.MoveSelected, move);
    }

    private void OnAttackButtonPressed()
    {
        if (Simulator is null)
            return;

        BuildMoveChoices(Simulator.Player.Battler);
        screen = ScreenType.MoveChoice;
        Update(Simulator.Battle.Phase);
    }
}
