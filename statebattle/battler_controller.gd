extends Node
class_name BattlerController

@export var battler: Battler


func _ready() -> void:
	battler.turn_started.connect(_on_turn_started)

func _on_turn_started() -> void:
	set_current_move(battler.moves.pick_random())

func set_current_move(move: Move):
	battler.current_move = move
	print("%s chose move %s" % [battler.name, move.name])
