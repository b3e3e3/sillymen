extends Resource
class_name Move

@export var name: String = "Move"
@export var animation_name: StringName


func get_animation() -> StringName:
	return animation_name if animation_name.begins_with("moves/") else "moves/%s" % animation_name

func do(battler: Battler) -> void:
	pass
