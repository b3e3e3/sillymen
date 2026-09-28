extends Resource
class_name MoveResult

enum TargetType {
	SELF,
	OTHER,
}

@export var _animation_name: StringName
#@export var debug_damage: int = 10
@export var chance: float = 1.0
@export var target_type := TargetType.OTHER

var affected_targets: Array[BattlerController]


func has_animation() -> bool:
	return _animation_name != &""

func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("status/%s" % _animation_name)
	
func get_valid_targets(user: Battler, battlers: Array[Battler]) -> Array[Battler]:
	match target_type:
		TargetType.SELF:
			return battlers.filter(func(b): return user == b)
		TargetType.OTHER:
			return battlers.filter(func(b): return user != b)
	return []
	
func get_result_message() -> String:
	return ""
	
func get_repeat_count() -> int:
	return 1

func can_apply() -> bool:
	return randf() <= chance

### Apply the move result, and return affected controllers
func apply(_controller: BattlerController, _target: BattlerController) -> bool:
	return false
