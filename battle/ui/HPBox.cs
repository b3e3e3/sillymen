using Godot;

namespace Sillymen;

[GlobalClass]
public partial class HPBox : Control
{
    [Export]
    public BattlerController controller;

    private RichTextLabel nameLabel;
    private RichTextLabel levelNumberLabel;
    private RichTextLabel hpNumberLabel;
    private ProgressBar hpBar;

    public HPBox() { }

    public HPBox(BattlerController controller = null)
    {
        if (controller != null)
        {
            this.controller = controller;
        }
    }

    public override void _Ready()
    {
        nameLabel = GetNode<RichTextLabel>("%NameLabel");
        levelNumberLabel = GetNode<RichTextLabel>("%LevelNumberLabel");
        hpNumberLabel = GetNode<RichTextLabel>("%HPNumberLabel");
        hpBar = GetNode<ProgressBar>("%HPBar");

        Update();
    }

    public void Update()
    {
        nameLabel.Text = controller.Battler.Name;
        levelNumberLabel.Text = controller.Battler.Level.ToString();
        hpNumberLabel.Text = controller.Battler.CurrentHp.ToString();
        hpBar.Value = (float)controller.Battler.CurrentHp / controller.Battler.MaxHp;
    }
}
