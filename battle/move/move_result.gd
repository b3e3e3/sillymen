extends Resource
class_name MoveResult

@export var _animation_name: StringName
@export var debug_damage: int = 10

func has_animation() -> bool:
	return _animation_name != &""

func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("status/%s" % _animation_name)

func apply(_controller: BattlerController, target: BattlerController) -> void:
	target.current_hp -= debug_damage # HACK
