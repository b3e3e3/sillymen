@tool
extends Control
class_name Battler

signal flash_started
signal flash_finished
signal move_started(move: Move)
signal move_finished


enum State {
	IDLE,
	PLAYER_CHOICE,
	MOVE,
	FLASH,
}

const flash_time = 0.14

@onready var player: AnimationPlayer = $AnimationPlayer

@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@export var moves: Array[Move] = []

var state := State.IDLE

var current_move: Move = null


func _ready() -> void:
	#player.animation_changed.connect(_on_animation_changed)
	player.play(&"RESET")
	switch_primary_sprite()

func flash(times = 3) -> void:
	state = State.FLASH
	flash_started.emit()
	
	for i in range(times):
		visible = false
		await get_tree().create_timer(flash_time/2).timeout
		visible = true
		await get_tree().create_timer(flash_time/2).timeout
	
	state = State.IDLE
	flash_finished.emit()
	
func take_turn() -> void:
	state = State.PLAYER_CHOICE

func do_move(move: Move) -> void:
	if current_move == null:
		push_error("No current move on battler %s" % name)
	
	state = State.MOVE
	current_move.do(self)
	move_started.emit(move)
	
	if move.has_animation():
		player.play(move.get_animation())
		await player.animation_finished
		
	player.play(&"RESET")
	
	state = State.IDLE
	move_finished.emit()

func choose_move() -> Move:
	return null

func switch_primary_sprite() -> void:
	$Sprite.texture = primary_sprite

func switch_secondary_sprite() -> void:
	$Sprite.texture = secondary_sprite
