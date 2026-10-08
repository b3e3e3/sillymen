#nullable enable
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class BattleState : Resource
{
    [Signal]
    public delegate void TurnStartedEventHandler(Battler battler);
    [Signal]
    public delegate void TurnCompletedEventHandler();
   
    public enum Phase
    {
        START,
        CHOICE,
        MOVE,
        POST_MOVE,
        END,
    }

    [Export] public Battler? active_battler { get; set; }
    [Export] public Array<Battler> battlers { get; set; } = [];

    public Phase phase = Phase.START;
    private Phase next_phase;
    private int turn_index;

    public BattleState()
    {
        next_phase = phase;
    }

    public Array<Battler> get_living_battlers() =>
        [.. battlers.Where(b => !b.is_fainted())];

    public void next_battler()
    {
        if (battlers.Count == 0) return;
        turn_index = (turn_index + 1) % battlers.Count;
    }

    public void process_state()
    {
        phase = next_phase;

        if (phase == Phase.CHOICE && get_living_battlers().Count <= 1)
        {
            phase = Phase.END;
            next_phase = Phase.END;
        }

        switch (phase)
        {
            case Phase.START:
                turn_index = 0;
                next_phase = Phase.CHOICE;
                break;
            case Phase.CHOICE:
                while (battlers[turn_index].is_fainted())
                    next_battler();
                active_battler = battlers[turn_index];
                EmitSignal(SignalName.TurnStarted, active_battler);
                next_phase = Phase.MOVE;
                break;
            case Phase.MOVE:
                next_phase = Phase.POST_MOVE;
                break;
            case Phase.POST_MOVE:
                next_battler();
                EmitSignal(SignalName.TurnCompleted);
                next_phase = Phase.CHOICE;
                break;
            case Phase.END:
                break;
        }
    }
}