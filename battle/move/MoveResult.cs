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
    public enum ValidTargetType
    {
        Self,
        Other,
    }

    [Export]
    public StringName AnimationName { get; set; } = "";

    [Export]
    public float Chance { get; set; } = 1.0f;

    [Export]
    public ValidTargetType TargetType { get; set; } = ValidTargetType.Other;

    protected List<BattlerController> affectedTargets = [];

    public virtual bool HasAnimation() => AnimationName != (StringName)"";

    public virtual StringName GetAnimation() =>
        AnimationName.ToString().Contains('/') ? AnimationName : $"status/{AnimationName}";

    public virtual List<Battler> GetValidTargets(Battler user, Array<Battler> battlers) =>
        TargetType switch
        {
            ValidTargetType.Self => [.. from b in battlers where b == user select b],
            ValidTargetType.Other => [.. from b in battlers where b != user select b],
            _ => [],
        };

    public abstract bool Apply(BattlerController controller, BattlerController target);

    public virtual bool CanApply() => new Random().NextDouble() <= Chance;

    public virtual string? GetResultMessage() => null;

    public virtual string? GetHitMessage() => null;

    public virtual int GetRepeatCount() => 1;
}
