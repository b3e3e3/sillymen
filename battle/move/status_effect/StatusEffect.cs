#nullable enable
using Godot;

namespace Sillymen;

[GlobalClass]
public abstract partial class StatusEffect : MoveResult // TOOD: move away from MoveResult. interface?
{
    [Export]
    public string name = "Status Condition";

    [Export]
    public int max_turns = 1;
    protected int count = 0;

    public bool has_expired() => count >= max_turns;

    public override StringName get_animation() =>
        _animation_name.ToString().Contains('/') ? _animation_name : $"status/{_animation_name}";
}
