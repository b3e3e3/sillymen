extends Resource
class_name Move

@export var name: String = "Move"
@export var _animation_name: StringName
@export var results: Array[MoveResult]


func get_animation() -> StringName:
	return _animation_name if _animation_name.contains("/") else StringName("moves/%s" % _animation_name)
	
func get_results() -> Array[MoveResult]:
	# TODO: if godot every fixes .map(...) to return a typed array, then use that instead lol
	var _results: Array[MoveResult] = []
	for r in results:
		_results.append(r.duplicate())
	return _results
