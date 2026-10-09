using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class TypeChart : Resource
{
    [Export]
    public Godot.Collections.Array<BattleType> Types { get; set; } = [];

    public TypeChart()
    {
        foreach (var type in Enum.GetValues<BattleType.TypeKind>())
        {
            if (Types.Any(bt => bt.Type == type))
                continue;
            Types.Add(new BattleType { Type = type });
        }
    }

    public float GetMultiplier(BattleType.TypeKind attacking, BattleType.TypeKind defending)
    {
        foreach (var bt in Types)
        {
            if (bt.Type != attacking)
                continue;
            return bt.Effectiveness.GetValueOrDefault(defending, 1.0f);
        }
        return 1.0f;
    }

    public float GetMultipliers(BattleType.TypeKind attacking, List<BattleType.TypeKind> defenders)
    {
        var total_mult = 1.0f;
        foreach (var defending in defenders)
        {
            if (defending == BattleType.TypeKind.NONE)
                continue;

            var mult = GetMultiplier(attacking, defending);
            total_mult *= mult;

            GD.Print($"{attacking} eff against {defending}? {mult}");
        }
        return total_mult;
    }
}
