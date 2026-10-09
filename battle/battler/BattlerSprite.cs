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
    public Battler? Battler { get; set; }

    [ExportCategory("Sprites")]
    [Export]
    public Texture2D? PrimarySprite { get; set; }

    [Export]
    public Texture2D? SecondarySprite { get; set; }

    public TextureRect? Texture { get; set; }
    public AnimationPlayer? AnimationPlayer { get; set; }

    public BattlerSprite() { }

    public BattlerSprite(Battler battler)
    {
        Battler = battler;
    }

    public override void _Ready()
    {
        PrimarySprite = Battler?.PrimarySprite; // used to be ??=
        SecondarySprite = Battler?.SecondarySprite; // used to be ??=

        AnimationPlayer ??= GetNode<AnimationPlayer>("AnimationPlayer");
        // if (texture == null)...

        SwitchPrimarySprite();
    }

    public void TriggerHitFrame() => EmitSignal(SignalName.HitFrame);

    public async Task PlayAnimation(StringName anim)
    {
        if (AnimationPlayer == null)
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
