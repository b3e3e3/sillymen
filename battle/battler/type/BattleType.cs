using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattleType : Resource
{
    public enum TypeKind
    {
        NORMAL,
        CRAZY,
        SCARY,
        SILLY,
        NONE,
    }

    [Export]
    public TypeKind Type { get; set; }

    [Export]
    public Godot.Collections.Dictionary<TypeKind, float> Effectiveness { get; set; } = [];
}
