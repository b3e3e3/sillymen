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
    public string name;

    [Export]
    public int level = 1;

    [Export]
    public int max_hp = 100;

    [ExportCategory("Type")]
    [Export]
    public BattleType.Type primary_type = BattleType.Type.NORMAL;

    [Export]
    public BattleType.Type secondary_type = BattleType.Type.NONE;

    [ExportCategory("Sprites")]
    [Export]
    public Texture2D primary_sprite { get; set; }

    [Export]
    public Texture2D secondary_sprite { get; set; }

    [ExportCategory("Moves")]
    [Export]
    public Array<Move> moves { get; set; } = [];

    [ExportCategory("Stats")]
    [Export]
    public Stat attack { get; set; }

    [Export]
    public Stat defense { get; set; }

    [Export]
    public Stat speed { get; set; }

    public Stat critical_rate { get; set; }

    private int _current_hp = -1;

    public int current_hp
    {
        get => _current_hp;
        set { _current_hp = Math.Clamp(value, 0, max_hp); }
    }

    public void initialize()
    {
        _current_hp = max_hp;

        attack.stage = 0;
        defense.stage = 0;
        speed.stage = 0;

        critical_rate = new(speed.base_value / 2);
        GD.Print($"Initialized {name}!");
    }

    public bool is_fainted() => current_hp == 0;

    public float GetCriticalChance() => critical_rate.value / 256;

    public List<BattleType.Type> get_types() => [primary_type, secondary_type];
}
