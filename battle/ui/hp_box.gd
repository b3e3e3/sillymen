extends Control
class_name HPBox

@export var controller: BattlerController


func _ready() -> void:
	update()

func update() -> void:
	%NameLabel.text = controller.battler.name
	%LevelNumberLabel.text = String.num_uint64(controller.battler.level)
	%HPNumberLabel.text = String.num_uint64(controller.current_hp)
	%HPBar.value = float(controller.current_hp) / controller.battler.max_hp
