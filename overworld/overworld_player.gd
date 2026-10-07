extends Node2D
class_name OverworldPlayer

const MOVE_TIME := 0.35
const WALK_BUFFER_TIME := 0.1

enum State {
	IDLE,
	MOVING,
}

@export var map: TileMapLayer

var state := State.IDLE
var direction: Vector2
var moving := false
var tile_size := 16
var _turn_counter := 0.0


func move() -> void:
	if not direction: return
	if moving: return
	
	moving = true
	
	var tween := create_tween()
	tween.tween_property(self, "position", position + direction * tile_size, MOVE_TIME)
	await tween.finished
	
	moving = false
	
func turn(dir: Vector2) -> void:
	direction = dir
	
func _physics_process(delta: float) -> void:
	var hor := Input.get_axis("move_left", "move_right")
	var ver := Input.get_axis("move_up", "move_down")
	
	match state:
		State.IDLE:
			var _dir := Vector2.ZERO
			if hor:
				_dir = hor * Vector2.RIGHT
			elif ver:
				_dir = ver * Vector2.DOWN
			
			turn(_dir)
			
			if _dir != Vector2.ZERO:
				_turn_counter += delta
				if _turn_counter >= WALK_BUFFER_TIME:
					move()
			else:
				_turn_counter = 0.0
			
