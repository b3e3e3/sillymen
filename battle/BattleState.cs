#nullable enable
using System.Linq;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattleState : Resource
{
    [Signal]
    public delegate void TurnStartedEventHandler(Battler battler);

    [Signal]
    public delegate void TurnCompletedEventHandler();

    public enum BattlePhase
    {
        Start,
        Choice,
        Move,
        PostMove,
        End,
    }

    [Export]
    public Battler? ActiveBattler { get; set; }

    [Export]
    public Godot.Collections.Array<Battler> Battlers { get; set; } = [];

    public BattlePhase Phase { get; set; } = BattlePhase.Start;
    private BattlePhase nextPhase;
    private int turnIndex;

    public BattleState()
    {
        nextPhase = Phase;
    }

    public Godot.Collections.Array<Battler> GetLivingBattlers() => [.. Battlers.Where(b => !b.IsFainted())];

    public void NextBattler()
    {
        if (Battlers.Count == 0)
            return;
        turnIndex = (turnIndex + 1) % Battlers.Count;
    }

    public void ProcessState()
    {
        Phase = nextPhase;

        if (Phase == BattlePhase.Choice && GetLivingBattlers().Count <= 1)
        {
            Phase = BattlePhase.End;
            nextPhase = BattlePhase.End;
        }

        switch (Phase)
        {
            case BattlePhase.Start:
                turnIndex = 0;
                nextPhase = BattlePhase.Choice;
                break;
            case BattlePhase.Choice:
                while (Battlers[turnIndex].IsFainted())
                    NextBattler();
                ActiveBattler = Battlers[turnIndex];
                EmitSignal(SignalName.TurnStarted, ActiveBattler);
                nextPhase = BattlePhase.Move;
                break;
            case BattlePhase.Move:
                nextPhase = BattlePhase.PostMove;
                break;
            case BattlePhase.PostMove:
                NextBattler();
                EmitSignal(SignalName.TurnCompleted);
                nextPhase = BattlePhase.Choice;
                break;
            case BattlePhase.End:
                break;
            default:
                break;
        }
    }
}
