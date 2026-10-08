using System.Linq;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class Move : Resource
{
    [Export] public string name = "Move";
    [Export] public BattleType.Type type;
    [Export] private StringName _animation_name;
    private Array<MoveResult> _results { get; set; }

    [Export]
    public Array<MoveResult> results
    {
        get => [.. (from r in results select (r.Duplicate())).OfType<MoveResult>()];
        set => _results = value;
    }

    public StringName get_animation() => _animation_name.ToString().Contains('/') ? _animation_name : $"moves/{_animation_name}";
}