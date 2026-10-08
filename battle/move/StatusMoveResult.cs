#nullable enable
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class StatusMoveResult : MoveResult
{
    [Export]
    public StatusEffect? status_effect;
    private BattlerController? appliedTo;

    public override string? get_result_message()
    {
        if (appliedTo == null)
            return null;
        return $"{appliedTo.battler.name} contracted {status_effect?.name}!";
    }

    public override bool apply(BattlerController controller, BattlerController target)
    {
        if (!can_apply())
            return false;
        if (status_effect == null)
            return false;

        target.status_effects.Add((StatusEffect)status_effect.Duplicate());
        appliedTo = target;

        return true;
    }
}
