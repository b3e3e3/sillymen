extends Resource
class_name Battler

signal turn_started

@export_category("General")
@export var name: String

@export_category("Sprites")
@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@export_category("Moves")
@export var moves: Array[Move]

var current_move: Move = null


func take_turn(state: BattleState) -> void:
	#current_move = moves.pick_random()
	turn_started.emit()
	current_move.do(self)
