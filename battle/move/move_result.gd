extends Resource
class_name MoveResult

@export var _animation_name: StringName
#@export var debug_damage: int = 10
@export var chance: float = 1.0

var affected_targets: Array[BattlerController]


func has_animation() -> bool:
	return _animation_name != &""

func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("status/%s" % _animation_name)

func can_apply() -> bool:
	return randf() <= chance

### Apply the move result, and return affected controllers
func apply(_controller: BattlerController, _target: BattlerController) -> bool:
	return false
