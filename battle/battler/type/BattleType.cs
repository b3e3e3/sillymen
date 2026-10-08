using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class BattleType : Resource
{
    public enum Type
    {
        NORMAL,
        CRAZY,
        SCARY,
        SILLY,
        NONE,
    }
    [Export] public Type type;
    [Export] public Dictionary<Type, float> effectiveness;
}