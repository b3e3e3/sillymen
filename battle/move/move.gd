extends Resource
class_name Move

@export var name: String = "Move"
@export var _animation_name: StringName
@export var results: Array[MoveResult]
@export var target_resolver := TargetResolver.new()


func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("moves/%s" % _animation_name)
	
func get_results() -> Array[MoveResult]:
	return results
