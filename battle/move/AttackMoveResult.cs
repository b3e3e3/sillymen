#nullable enable
using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class AttackMoveResult : MoveResult
{
    private const float debugCritMultiplier = 2.0f;

    [Export]
    public int Power { get; set; }

    [Export]
    public int MinHits { get; set; } = 1;

    [Export]
    public int MaxHits { get; set; } = 1;

    private int landedHits = 0;
    private bool lastHitCrit = false;

    private RandomNumberGenerator rng = new();

    private int CalculateDamage(BattlerController controller, BattlerController target)
    {
        int dmg;

        var crit = lastHitCrit ? debugCritMultiplier : 1.0f;
        var level = controller.Battler.Level;
        var attack = controller.Battler.Attack.Value;
        var defense = target.Battler.Defense.Value;
        var level_divisor = 32.0f;
        var effectiveness = controller.Simulator.TypeChart?.GetMultipliers(
            controller.CurrentMove.Type,
            target.Battler.GetTypes()
        );

        GD.Print(
            $"level={level} crit={crit} power={Power} atk={attack} def={defense} eff={effectiveness}"
        );

        dmg = Mathf.RoundToInt(
            ((2.0f * level * crit / 5.0f) + 2.0f) * Power * (attack / defense) / level_divisor
                + 2.0f
        );

        var random = dmg != 1 ? rng.RandfRange(0.85f, 1.0f) : 1.0f;

        dmg = Mathf.CeilToInt(dmg * random);

        return dmg;
    }

    public override int GetRepeatCount() => rng.RandiRange(MinHits, MaxHits);

    public virtual bool IsCritical(BattlerController controller) =>
        rng.Randf() < controller.Battler.GetCriticalChance();

    public override bool Apply(BattlerController controller, BattlerController target)
    {
        if (!CanApply())
            return false;
        lastHitCrit = IsCritical(controller);
        GD.Print($"is crit? {lastHitCrit}");

        target.Battler.CurrentHp -= CalculateDamage(controller, target);
        affectedTargets.Add(target);
        landedHits++;

        return true;
    }

    public override string? GetHitMessage() => lastHitCrit ? "Critical hit!" : null;

    public override string? GetResultMessage() => MaxHits > 1 ? $"Hit {landedHits} time(s)!" : null;
}
