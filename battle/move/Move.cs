using System.Linq;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class Move : Resource
{
    [Export]
    public string Name { get; set; } = "Move";

    [Export]
    public BattleType.TypeKind Type { get; set; }

    [Export]
    public StringName AnimationName { get; set; }

    [Export]
    public Godot.Collections.Array<MoveResult> Results
    {
        get => [.. (from r in field select (r.Duplicate())).OfType<MoveResult>()];
        set;
    } = [];

    public StringName GetAnimation() =>
        AnimationName.ToString().Contains('/') ? AnimationName : $"moves/{AnimationName}";
}
