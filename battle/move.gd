extends Resource
class_name Move

@export var animation: StringName = ""
@export var display_name: String = "Move"

func has_animation() -> bool:
	return animation.strip_edges() != ""
	
func get_animation() -> StringName:
	return animation if animation.contains("moves/") else "moves/" + animation

func do(battler: Battler) -> void:
	print("%s did %s!" % [battler.name, display_name])
