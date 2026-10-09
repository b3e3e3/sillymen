using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Sillymen;

[GlobalClass]
public partial class Battler : Resource
{
    [ExportCategory("General")]
    [Export]
    public string Name { get; set; }

    [Export]
    public int Level { get; set; } = 1;

    [Export]
    public int MaxHp { get; set; } = 100;

    [ExportCategory("Type")]
    [Export]
    public BattleType.TypeKind PrimaryType { get; set; } = BattleType.TypeKind.NORMAL;

    [Export]
    public BattleType.TypeKind SecondaryType { get; set; } = BattleType.TypeKind.NONE;

    [ExportCategory("Sprites")]
    [Export]
    public Texture2D PrimarySprite { get; set; }

    [Export]
    public Texture2D SecondarySprite { get; set; }

    [ExportCategory("Moves")]
    [Export]
    public Array<Move> Moves { get; set; } = [];

    [ExportCategory("Stats")]
    [Export]
    public Stat Attack { get; set; }

    [Export]
    public Stat Defense { get; set; }

    [Export]
    public Stat Speed { get; set; }

    public Stat CriticalRate { get; set; }

    public int CurrentHp
    {
        get;
        set => field = Math.Clamp(value, 0, MaxHp);
    }

    public void Initialize()
    {
        CurrentHp = MaxHp;

        Attack.Stage = 0;
        Defense.Stage = 0;
        Speed.Stage = 0;

        CriticalRate = new(Speed.BaseValue / 2);
        GD.Print($"Initialized {Name}!");
    }

    public bool IsFainted() => CurrentHp == 0;

    public float GetCriticalChance() => CriticalRate.Value / 256;

    public List<BattleType.TypeKind> GetTypes() => [PrimaryType, SecondaryType];
}
