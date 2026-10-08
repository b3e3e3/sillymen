#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public abstract partial class MoveResult : Resource
{
    public enum TargetType
    {
        SELF,
        OTHER,
    }

    [Export] protected StringName _animation_name = "";
    [Export] public float chance = 1.0f;
    [Export] public TargetType target_type = TargetType.OTHER;

    protected List<BattlerController> affected_targets = [];

    public virtual bool has_animation() => _animation_name != (StringName)"";
    public virtual StringName get_animation() => _animation_name.ToString().Contains('/') ? _animation_name : $"status/{_animation_name}";
    public virtual List<Battler> get_valid_targets(Battler user, Array<Battler> battlers) => target_type switch
    {
        TargetType.SELF => [.. from b in battlers where b == user select b],
        TargetType.OTHER => [.. from b in battlers where b != user select b],
        _ => [],
    };

    public abstract bool apply(BattlerController controller, BattlerController target);
    public virtual bool can_apply() => new Random().NextDouble() <= chance;
    public virtual string? get_result_message() => null;
    public virtual string? get_hit_message() => null;
    public virtual int get_repeat_count() => 1;
}