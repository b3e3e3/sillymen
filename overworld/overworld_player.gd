extends Node2D
class_name OverworldPlayer

enum State {
	IDLE,
	MOVING,
}
var state := State.IDLE

@export var map: TileMapLayer

var local_position := Vector2.ZERO

func move(input: Vector2i) -> void:
	var tween := get_tree().create_tween()
	var target_pos := map.map_to_local(map.local_to_map(global_position) + input)
	tween.tween_property(self, "global_position", target_pos, 0.3)
	print("Moving from %s to %s" % [global_position, target_pos])
	state = State.MOVING
	await tween.finished
	state = State.IDLE

func _physics_process(delta: float) -> void:
	var input := Input.get_vector("move_left", "move_right", "move_up", "move_down") as Vector2i
	match state:
		State.IDLE:
			if not input == Vector2i.ZERO:
				move(input)
			
