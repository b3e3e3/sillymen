using System.Threading.Tasks;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class BattlerController : Node
{
    [Export]
    public Battler Battler { get; set; }
    public Move CurrentMove { get; set; } = null;
    public Array<StatusEffect> StatusEffects { get; set; } = [];
    public BattleSimulator Simulator { get; protected set; }

    public BattlerController() { }

    public BattlerController(Battler defaultBattler)
    {
        Battler = defaultBattler;
    }

    public override void _Ready()
    {
        GD.Print("Controller ready...");
        Simulator = GetParent<BattleSimulator>();
        Battler.Initialize();
        GD.Print("Done!");
    }

    public void RemoveStatusEffect(StatusEffect e)
    {
        StatusEffects.Remove(e);
    }

    public virtual async Task<Move> ChooseMove( /*BattleState state*/
    ) => Battler.Moves.PickRandom();
}
