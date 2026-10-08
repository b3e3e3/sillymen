using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class TypeChart : Resource
{
    [Export] public Array<BattleType> types = [];


    public TypeChart()
    {
        foreach (var type in Enum.GetValues<BattleType.Type>())
        {
            if (types.Any(bt => bt.type == type)) continue;
            types.Add(new BattleType { type = type });
        }
    }

    public float get_multiplier(BattleType.Type attacking, BattleType.Type defending)
    {
        foreach (var bt in types)
        {
            if (bt.type != attacking) continue;
            return bt.effectiveness.GetValueOrDefault(defending, 1.0f);
        }
        return 1.0f;
    }

    public float get_multipliers(BattleType.Type attacking, List<BattleType.Type> defenders)
    {
        var total_mult = 1.0f;
        foreach (var defending in defenders)
        {
            if (defending == BattleType.Type.NONE) continue;

            var mult = get_multiplier(attacking, defending);
            total_mult *= mult;
            
            GD.Print($"{Enum.GetName(attacking)} eff against {Enum.GetName(defending)}? {mult}");
        }
        return total_mult;
    }
}