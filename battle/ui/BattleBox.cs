#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattleBox : Control
{
    public enum Screen
    {
        NORMAL,
        MOVE_CHOICE,
    }

    [Signal]
    public delegate void MoveSelectedEventHandler(Move move);

    [Export]
    public BattleSimulator? simulator;
    [Export]
    private Control? dialog_container;
    [Export]
    private RichTextLabel? dialog_label;
    [Export]
    private Control? choice_container;
    [Export]
    private Control? moves_container;

    private string message = "";
    public Screen screen = Screen.NORMAL;

    private void build_move_choices(Battler battler)
    {
        var children = moves_container?.GetChildren() ?? [];
        if (children.Count > 0) return; // TODO: if moves need to change during battle, dont do this

        var scene = GD.Load<PackedScene>("res://battle/ui/move_button.tscn");

        // foreach (var c in children)
        // {
        //     c.QueueFree();
        // }

        foreach (var move in battler.moves)
        {
            var button = scene.Instantiate<Button>();
            button.Text = move.name;
            button.Connect(Button.SignalName.Pressed, Callable.From(() =>
            {
                OnMoveButtonPressed(move);
            }), (uint)ConnectFlags.OneShot);
            Callable.From(() =>
            {
                moves_container?.AddChild(button);
            }).CallDeferred();
        }
    }

    public void show_message(string text)
    {
        message = text;
        if (simulator != null) update(simulator.battle.phase);
    }

    public string? get_message(BattleState state)
    {
        switch (state.phase)
        {
            case BattleState.Phase.CHOICE:
                if (simulator?.is_player_turn() == true)
                {
                    return $"What will {state.active_battler?.name} do?";
                }
                break;
            case BattleState.Phase.MOVE:
                GD.Print($"{state.active_battler?.name} is about to use a move!");

                var controller = simulator?.get_controller_for(state.active_battler);
                return $"{state.active_battler?.name} used {controller?.current_move.name}!";
            // case BattleState.Phase.POST_MOVE:
        }

        return null;
    }

    public void update(BattleState.Phase phase)
    {
        dialog_label?.Text = message; // get_message(state);

        switch (phase)
        {
            case BattleState.Phase.CHOICE:
                if (screen == Screen.NORMAL)
                {
                    dialog_container?.Visible = true;
                    choice_container?.Visible = simulator?.is_player_turn() == true;
                    moves_container?.Visible = false;
                }
                else if (screen == Screen.MOVE_CHOICE)
                {
                    dialog_container?.Visible = false;
                    choice_container?.Visible = false;
                    moves_container?.Visible = true;
                }
                break;
            case BattleState.Phase.MOVE:
            case BattleState.Phase.POST_MOVE:
                dialog_container?.Visible = true;
                choice_container?.Visible = false;
                moves_container?.Visible = false;
                break;
            default:
                dialog_container?.Visible = true;
                choice_container?.Visible = false;
                moves_container?.Visible = false;
                break;
        }
    }

    private void OnMoveButtonPressed(Move move)
    {
        screen = Screen.NORMAL;
        EmitSignal(SignalName.MoveSelected, move);
    }

    private void OnAttackButtonPressed()
    {
        if (simulator == null) return;

        build_move_choices(simulator.player.battler);
        screen = Screen.MOVE_CHOICE;
        update(simulator.battle.phase);
    }
}