using System.Threading.Tasks;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class PlayerBattlerController : BattlerController
{
    public override async Task<Move> choose_move(/*BattleState state*/)
    {
        return await simulator.battle_box.ToSignal<Move>(BattleBox.SignalName.MoveSelected);
    }
}