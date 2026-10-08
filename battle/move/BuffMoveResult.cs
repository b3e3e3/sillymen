using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BuffMoveResult : MoveResult
{
    public override bool apply(BattlerController controller, BattlerController target)
    {
        if (!can_apply()) return false;
        affected_targets.Add(target);
        // _at.stats[stat] += amount;

        return true;
    }
}