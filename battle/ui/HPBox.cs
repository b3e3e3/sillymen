using System;
using Godot;

namespace Sillymen;

[GlobalClass]
public partial class HPBox : Control
{
    [Signal]
    public delegate void HpChangeFinishedEventHandler(int currentHp);

    [Export]
    public double HpChangeSpeed { get; set; } = 20.0;

    [Export]
    public BattlerController Controller { get; set; }

    private RichTextLabel nameLabel;
    private RichTextLabel levelNumberLabel;
    private RichTextLabel hpNumberLabel;
    private ProgressBar hpBar;
    private int targetHpValue;
    private bool hpChanging = false;

    public HPBox() { }

    public HPBox(BattlerController controller = null)
    {
        Controller = controller ?? Controller;
    }

    public override async void _Ready()
    {
        nameLabel = GetNode<RichTextLabel>("%NameLabel");
        levelNumberLabel = GetNode<RichTextLabel>("%LevelNumberLabel");
        hpNumberLabel = GetNode<RichTextLabel>("%HPNumberLabel");
        hpBar = GetNode<ProgressBar>("%HPBar");

        Update();
        hpBar.Value = Controller.Battler.CurrentHp;
    }

    public void Update()
    {
        targetHpValue = Controller.Battler.CurrentHp;
        hpBar.MaxValue = Controller.Battler.MaxHp;
        // hpBar.Value = (float)controller.Battler.CurrentHp / controller.Battler.MaxHp;
        nameLabel.Text = Controller.Battler.Name;
        levelNumberLabel.Text = Controller.Battler.Level.ToString();
        hpNumberLabel.Text = Controller.Battler.CurrentHp.ToString();
    }

    public override void _Process(double delta)
    {
        ProcessHpBar(delta);
        if (hpChanging)
        {
            GD.Print(
                $"HP Changing! Target HP Value: {targetHpValue}. HP bar Value? {hpBar.Value}. Equal? {targetHpValue == hpBar.Value}"
            );
        }
    }

    public bool IsStable() => targetHpValue == hpBar.Value;

    private void ProcessHpBar(double delta)
    {
        if (IsStable())
        {
            if (hpChanging)
            {
                hpChanging = false;
                GD.Print("Done changing.");
                if (Controller is not null)
                    EmitSignal(SignalName.HpChangeFinished, Controller.Battler.CurrentHp);
            }
            return;
        }

        hpChanging = true;
        hpBar.Value = Math.Clamp(
            Mathf.MoveToward(hpBar.Value, targetHpValue, delta * HpChangeSpeed),
            hpBar.MinValue,
            hpBar.MaxValue
        );
    }
}
