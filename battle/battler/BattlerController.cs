using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class BattlerController : Node
{
    [Export]
    public Battler battler { get; set; }
    public Move current_move { get; set; } = null;
    public Array<StatusEffect> status_effects { get; set; } = [];
    public BattleSimulator simulator { get; protected set; }

    public BattlerController() { }

    public BattlerController(Battler default_battler)
    {
        battler = default_battler;
    }

    public override void _Ready()
    {
        GD.Print("Controller ready...");
        simulator = GetParent<BattleSimulator>();
        battler.initialize();
        GD.Print("Done!");
    }

    public void remove_status_effect(StatusEffect e)
    {
        status_effects.Remove(e);
    }

    public virtual async Task<Move> choose_move( /*BattleState state*/
    ) => battler.moves.PickRandom();
}
