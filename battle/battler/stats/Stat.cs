using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class Stat : Resource
{
    [Export] public float base_value;
    [Export] public float min_value { get; private set; } = 0.0f;
    [Export] public float max_value { get; private set; } = 255.0f;
    [Export] public int min_stage { get; private set; } = -4;
    [Export] public int max_stage { get; private set; } = 4;
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
                _cached_value = _calculate();
                _dirty = false;
            }
            return _cached_value;
        }
    }

    private int _stage = 0;
    private float _cached_value;
    private bool _dirty = true;

    public Stat() { }
    public Stat(float? stat_value)
    {
        base_value = stat_value ?? base_value;
    }

    private float _calculate()
    {
        var flat = 0.0f;
        var mult = 1.0f;
        // TODO: foreach (m in modifiers)...
        var staged = (base_value + flat) * get_stage_multiplier() * mult;
        return Math.Clamp(staged, min_value, max_value);
    }

    // Pokemon-style: +1 = 3/2, +2 = 4/2, -1 = 2/3, ...
    public float get_stage_multiplier() =>
        (float)(2.0 + Math.Max(stage, 0)) / (2.0f + Math.Max(-stage, 0));

    public void add_modifier(/**/)
    {
        _dirty = true;
        throw new NotImplementedException();
    }

    public void remove_modifier(/**/)
    {
        _dirty = true;
        throw new NotImplementedException();
    }

    public void change_stage(int by) => stage += by;
}