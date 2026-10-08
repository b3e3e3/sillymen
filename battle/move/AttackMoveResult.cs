#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class AttackMoveResult : MoveResult
{
    private const float DEBUG_CRIT_MULT = 2.0f;
    [Export]
    public int power;
    [Export]
    public int min_hits = 1;
    [Export]
    public int max_hits = 1;

    private int _landed_hits = 0;
    private bool _last_hit_crit = false;

    private RandomNumberGenerator rng = new();

    private int CalculateDamage(BattlerController controller, BattlerController target)
    {
        int dmg;

        var crit = _last_hit_crit ? DEBUG_CRIT_MULT : 1.0f;
        var level = controller.battler.level;
        var attack = controller.battler.attack.value;
        var defense = target.battler.defense.value;
        var level_divisor = 32.0f;
        var effectiveness = controller.simulator.type_chart.get_multipliers(controller.current_move.type, target.battler.get_types());

        Console.WriteLine($"level={level} crit={crit} power={power} atk={attack} def={defense} eff={effectiveness}");

        dmg = Mathf.RoundToInt(((2.0f * level * crit / 5.0f) + 2.0f) * power * (attack / defense) / level_divisor + 2.0f);

        var random = dmg != 1 ? rng.RandfRange(0.85f, 1.0f) : 1.0f;

        dmg = Mathf.CeilToInt(dmg * random);

        return dmg;
    }

    public override int get_repeat_count() => rng.RandiRange(min_hits, max_hits);

    public override bool apply(BattlerController controller, BattlerController target)
    {
        if (!can_apply()) return false;
        _last_hit_crit = rng.Randf() < controller.battler.critical_rate.value;

        target.battler.current_hp -= CalculateDamage(controller, target);
        affected_targets.Add(target);
        _landed_hits++;

        return true;
    }

    public override string? get_hit_message() => _last_hit_crit ? "Critical hit!" : null;

    public override string? get_result_message() => max_hits > 1 ? $"Hit {_landed_hits} time(s)!" : null;
}