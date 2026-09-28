extends Resource
class_name Move

@export var name: String = "Move"
@export var _animation_name: StringName
@export var result: MoveResult

@export_enum("Self", "Others") var valid_targets: String = "Others"


func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("moves/%s" % _animation_name)

func is_valid_target(user: Battler, target: Battler) -> bool:
	match valid_targets:
		"Self":
			return user == target
		"Others":
			return user != target
	return false
	
func get_valid_targets(user: Battler, battlers: Array[Battler]) -> Array[Battler]:
	return battlers.filter(func(b):
		return is_valid_target(user, b)
	)
