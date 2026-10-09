#nullable enable
using Godot;

namespace Sillymen;

[GlobalClass]
public abstract partial class StatusEffect : MoveResult // TOOD: move away from MoveResult. interface?
{
    [Export]
    public string Name { get; set; } = "Status Condition";

    [Export]
    public int MaxTurns { get; set; } = 1;
    public int count = 0;

    public bool HasExpired() => count >= MaxTurns;

    public override StringName GetAnimation() =>
        AnimationName.ToString().Contains('/') ? AnimationName : $"status/{AnimationName}";
}
