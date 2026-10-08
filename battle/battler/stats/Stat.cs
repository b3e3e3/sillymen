using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class Stat : Resource
{
    [Export]
    public float base_value;

    public float min_value { get; private set; } = 0.0f;

    public float max_value { get; private set; } = 255.0f;

    [Export]
    public int min_stage { get; private set; } = -4;

    [Export]
    public int max_stage { get; private set; } = 4;
    public int stage
    {
        get => _stage;
        set
        {
            _stage = Math.Clamp(value, min_stage, max_stage);
            _dirty = true;
        }
    }
    public float value
    {
        get
        {
            if (_dirty)
            {
                _cached_value = GetValueAtStage(0);
                _dirty = false;
            }
            return _cached_value;
        }
    }

    [ExportCategory("IVs/EVs")]
    [Export]
    public int intrinsic = 0;

    [Export]
    public int effort = 0;

    private int _stage = 0;
    private float _cached_value;
    private bool _dirty = true;

    public Stat() { }

    public Stat(float? stat_value)
    {
        base_value = stat_value ?? base_value;
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
    public float get_stage_multiplier(int atStage) =>
        (float)(2.0 + Math.Max(atStage, 0)) / (2.0f + Math.Max(-atStage, 0));

    public float get_stage_multiplier() => get_stage_multiplier(stage);

    public float GetValueAtStage(int extraStages)
    {
        var flat = intrinsic + effort;
        var mult = 1.0f;

        // TODO: foreach (m in modifiers)...

        var s = Math.Clamp(stage + extraStages, min_stage, max_stage);
        var staged = (base_value + flat) * get_stage_multiplier(s) * mult;
        GD.Print($"Value at stage: {Math.Clamp(staged, min_value, max_value)}");
        return Math.Clamp(staged, min_value, max_value);
    }

    public void add_modifier( /**/
    )
    {
        _dirty = true;
        throw new NotImplementedException();
    }

    public void remove_modifier( /**/
    )
    {
        _dirty = true;
        throw new NotImplementedException();
    }
}
