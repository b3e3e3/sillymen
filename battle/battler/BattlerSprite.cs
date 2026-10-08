#nullable enable
using System.Threading.Tasks;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class BattlerSprite : Control
{
    [Signal]
    public delegate void HitFrameEventHandler();

    [Export]
    public Battler? battler { get; set; }

    [ExportCategory("Sprites")]
    [Export]
    public Texture2D? primary_sprite { get; set; }

    [Export]
    public Texture2D? secondary_sprite { get; set; }

    public TextureRect? texture;
    public AnimationPlayer? animation_player;

    public BattlerSprite() { }

    public BattlerSprite(Battler battler)
    {
        this.battler = battler;
    }

    public override void _Ready()
    {
        primary_sprite = battler?.primary_sprite; // used to be ??=
        secondary_sprite = battler?.secondary_sprite; // used to be ??=

        animation_player ??= GetNode<AnimationPlayer>("AnimationPlayer");
        // if (texture == null)...

        switch_primary_sprite();
    }

    public void trigger_hit_frame() => EmitSignal(SignalName.HitFrame);

    public async Task play_animation(StringName anim)
    {
        if (animation_player == null)
            return;
        if (!animation_player.HasAnimation(anim))
        {
            GD.PushWarning($"No animation '{anim}' found.");
            return;
        }

        animation_player.Play(anim);
        await ToSignal(animation_player, AnimationMixer.SignalName.AnimationFinished);
        animation_player.Play("RESET");
        await ToSignal(animation_player, AnimationMixer.SignalName.AnimationFinished);

        // # WIP: hit frames, but theyre currently unused. works tho
        // #var resolved := [false] # HACK: array so lambda captures
        // #var on_hit := func():
        //     #if resolved[0]: return
        //     #resolved[0] = true
        //     #_move_resolved.emit()
        // #
        // #var on_finished := func(_anim):
        //     #if resolved[0]: return
        //     #resolved[0] = true
        //     #_move_resolved.emit()
        //     #
        // #hit_frame.connect(on_hit)
        // #animation_player.animation_finished.connect(on_finished)
        // #
        // #animation_player.play(anim)
        // #
        // #await _move_resolved
        // #
        // #hit_frame.disconnect(on_hit)
        // #animation_player.animation_finished.disconnect(on_finished)
    }

    public void switch_primary_sprite() => texture?.Texture = primary_sprite;

    public void switch_secondary_sprite() => texture?.Texture = secondary_sprite;
}
