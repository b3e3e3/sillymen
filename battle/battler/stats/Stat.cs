using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class Stat : Resource
{
    [Export]
    public float BaseValue { get; set; }

    public float MinValue { get; private set; } = 0.0f;

    public float MaxValue { get; private set; } = 255.0f;

    [Export]
    public int MinStage { get; private set; } = -4;

    [Export]
    public int MaxStage { get; private set; } = 4;
    public int Stage
    {
        get;
        set
        {
            field = Math.Clamp(value, MinStage, MaxStage);
            dirty = true;
        }
    } = 0;
    public float Value
    {
        get
        {
            if (dirty)
            {
                field = GetValueAtStage(0);
                dirty = false;
            }
            return field;
        }
        private set;
    }

    [ExportCategory("IVs/EVs")]
    [Export]
    public int intrinsic = 0;

    [Export]
    public int effort = 0;
    private bool dirty = true;

    public Stat() { }

    public Stat(float? statValue)
    {
        BaseValue = statValue ?? BaseValue;
    }

    // private float _calculate(int atStage)
    // {
    //     var flat = 0.0f;
    //     var mult = 1.0f;
    //     // TODO: foreach (m in modifiers)...
    //     var staged = (base_value + flat) * get_stage_multiplier(atStage) * mult;
    //     return Math.Clamp(staged, min_value, max_value);
    // }

    // Pokemon-style: +1 = 3/2, +2 = 4/2, -1 = 2/3, ...
    public static float GetStageMultiplier(int atStage) =>
        (float)(2.0 + Math.Max(atStage, 0)) / (2.0f + Math.Max(-atStage, 0));

    public float GetStageMultiplier() => GetStageMultiplier(Stage);

    public float GetValueAtStage(int extraStages)
    {
        var flat = intrinsic + effort;
        var mult = 1.0f;

        // TODO: foreach (m in modifiers)...

        var s = Math.Clamp(Stage + extraStages, MinStage, MaxStage);
        var staged = (BaseValue + flat) * GetStageMultiplier(s) * mult;
        GD.Print($"Value at stage: {Math.Clamp(staged, MinValue, MaxValue)}");
        return Math.Clamp(staged, MinValue, MaxValue);
    }

    public void AddModifier( /**/
    )
    {
        dirty = true;
        throw new NotImplementedException();
    }

    public void RemoveModifier( /**/
    )
    {
        dirty = true;
        throw new NotImplementedException();
    }
}
