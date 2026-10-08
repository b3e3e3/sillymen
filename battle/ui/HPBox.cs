using Godot;

namespace Sillymen;

[GlobalClass]
public partial class HPBox : Control
{
    [Export]
    public BattlerController controller;

    private RichTextLabel name_label;
    private RichTextLabel level_number_label;
    private RichTextLabel hp_number_label;
    private ProgressBar hp_bar;

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
        name_label = GetNode<RichTextLabel>("%NameLabel");
        level_number_label = GetNode<RichTextLabel>("%LevelNumberLabel");
        hp_number_label = GetNode<RichTextLabel>("%HPNumberLabel");
        hp_bar = GetNode<ProgressBar>("%HPBar");

        update();
    }

    public void update()
    {
        name_label.Text = controller.battler.name;
        level_number_label.Text = controller.battler.level.ToString();
        hp_number_label.Text = controller.battler.current_hp.ToString();
        hp_bar.Value = (float)controller.battler.current_hp / controller.battler.max_hp;
    }
}
