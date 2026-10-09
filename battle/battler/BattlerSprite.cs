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
    public BattlerController? Controller { get; set; }

    public Texture2D? PrimarySprite => Controller?.Battler.PrimarySprite;
    public Texture2D? SecondarySprite => Controller?.Battler.SecondarySprite;

    public TextureRect? Texture { get; set; }
    public AnimationPlayer? AnimationPlayer { get; set; }

    public override void _Ready()
    {
        Texture = GetNode<TextureRect>("BattlerTexture");
        AnimationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
        // if (texture is null)...

        SwitchPrimarySprite();
    }

    public void TriggerHitFrame() => EmitSignal(SignalName.HitFrame);

    public async Task PlayAnimation(StringName anim)
    {
        if (AnimationPlayer is null)
            return;
        if (!AnimationPlayer.HasAnimation(anim))
        {
            GD.PushWarning($"No animation '{anim}' found.");
            return;
        }

        AnimationPlayer.Play(anim);
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);
        AnimationPlayer.Play("RESET");
        await ToSignal(AnimationPlayer, AnimationMixer.SignalName.AnimationFinished);

        GD.Print("Done playing anim 2");
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

    public void SwitchPrimarySprite() => Texture?.Texture = PrimarySprite;

    public void SwitchSecondarySprite() => Texture?.Texture = SecondarySprite;
}
