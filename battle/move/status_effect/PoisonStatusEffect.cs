#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class PoisonStatusEffect : StatusEffect
{
    private const float DMG_MULT = 1.0f / 16;

    private BattlerController? _target_controller;

    private int get_damage(BattlerController target) => Math.Max(1, Mathf.RoundToInt(target.battler.max_hp * DMG_MULT));

    public override bool apply(BattlerController controller, BattlerController target)
    {
        if (controller.battler.current_hp == 0) return false;
        controller.battler.current_hp -= get_damage(controller);
        
        _target_controller = controller;
        count++;
        return true;
    }

    public override string? get_result_message() => $"{_target_controller?.battler.name} took damage from {name}!";
}