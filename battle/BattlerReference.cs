#nullable enable
namespace Sillymen;

public record BattlerReference
{
    public required Battler Battler { get; init; }
    public required BattlerController Controller { get; init; }
    public BattlerSprite? Sprite { get; set; }
    public HPBox? HpBox { get; set; }
}
