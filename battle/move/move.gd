extends Resource
class_name Move

@export var name: String = "Move"
@export var _animation_name: StringName
@export var results: Array[MoveResult]


func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("moves/%s" % _animation_name)
	
func get_results() -> Array[MoveResult]:
	var typed: Array[MoveResult] = []
	typed.assign(results.map(func(e): return e.duplicate()))
	return typed
