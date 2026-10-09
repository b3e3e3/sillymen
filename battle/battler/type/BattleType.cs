using Godot;
using Godot.Collections;

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
    public Dictionary<TypeKind, float> Effectiveness { get; set; } = [];
}
