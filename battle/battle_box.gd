extends PanelContainer

@onready var battle: Battle = owner
@onready var choice_panel: Control = $HBoxContainer/ChoicePanel

#func _process(delta: float) -> void:
	#choice_panel.visible = battle.phase == Battle.Phase.CHOICE
