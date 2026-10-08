# extends MoveResult
# class_name StatusEffectLegacy

# @export var name: String = "Status Condition"
# @export var max_turns: int = 1
# var count: int = 0

# func has_expired() -> bool:
# 	return count >= max_turns

# func get_animation() -> StringName:
# 	return _animation_name if _animation_name.contains("/") else StringName("status/%s" % [_animation_name])
