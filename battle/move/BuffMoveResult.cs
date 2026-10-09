using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BuffMoveResult : MoveResult
{
    public override bool Apply(BattlerController controller, BattlerController target)
    {
        if (!CanApply())
            return false;
        affectedTargets.Add(target);
        // _at.stats[stat] += amount;

        return true;
    }
}
