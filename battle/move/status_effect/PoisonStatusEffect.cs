#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class PoisonStatusEffect : StatusEffect
{
    private const float damageMult = 1.0f / 16;

    private BattlerController? targetController;

    private static int GetDamage(BattlerController target) =>
        Math.Max(1, Mathf.RoundToInt(target.Battler.MaxHp * damageMult));

    public override bool Apply(BattlerController controller, BattlerController target)
    {
        if (controller.Battler.CurrentHp == 0)
            return false;
        controller.Battler.CurrentHp -= GetDamage(controller);

        targetController = controller;
        count++;
        return true;
    }

    public override string? GetResultMessage() =>
        $"{targetController?.Battler.Name} took damage from {Name}!";
}
