using System.Threading.Tasks;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class PlayerBattlerController : BattlerController
{
    public override async Task<Move> ChooseMove( /*BattleState state*/
    )
    {
        return await Simulator.BattleBox.ToSignal<Move>(BattleBox.SignalName.MoveSelected);
    }
}
