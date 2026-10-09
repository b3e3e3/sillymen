#nullable enable
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class StatusMoveResult : MoveResult
{
    [Export]
    public StatusEffect? StatusEffect { get; set; }
    private BattlerController? appliedTo;

    public override string? GetResultMessage() =>
        appliedTo == null ? null : $"{appliedTo.Battler.Name} contracted {StatusEffect?.Name}!";

    public override bool Apply(BattlerController controller, BattlerController target)
    {
        if (!CanApply())
            return false;
        if (StatusEffect == null)
            return false;

        target.StatusEffects.Add((StatusEffect)StatusEffect.Duplicate());
        appliedTo = target;

        return true;
    }
}
