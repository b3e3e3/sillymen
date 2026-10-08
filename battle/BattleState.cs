// using System;
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

    [Export] public Battler active_battler { get; set; }
    [Export] public Array<Battler> battlers { get; set; } = [];

    public Phase phase = Phase.START;
    private Phase next_phase;
    private Queue<Battler> _turn_queue;
    private Queue<Battler> turn_queue => _turn_queue ??= new Queue<Battler>(battlers);

    public BattleState()
    {
        next_phase = phase;
    }

    public Array<Battler> get_living_battlers() => (from b in battlers where !b.is_fainted() select b) as Array<Battler>;
    public void next_battler() => turn_queue.Enqueue(turn_queue.Dequeue());
    public void process_state()
    {
        var phase = next_phase;

        if (phase == Phase.CHOICE && get_living_battlers().Count <= 1)
        {
            phase = Phase.END;
            next_phase = Phase.END;
        }

        switch (phase)
        {
            case Phase.START:
                next_phase = Phase.CHOICE;
                break;
            case Phase.CHOICE:
                while (battlers[0].is_fainted())
                    next_battler();
                active_battler = battlers[0];
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