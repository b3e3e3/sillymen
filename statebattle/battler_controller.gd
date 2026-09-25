extends Node
class_name BattlerController

signal turn_taken

@export var battler: Battler
@onready var simulator: BattleSimulator = get_parent() # HACK


func _ready() -> void:
	battler.turn_taken.connect(turn_taken.emit)
	
func _exit_tree() -> void:
	battler.turn_taken.disconnect(turn_taken.emit)

func set_current_move(move: Move):
	battler.current_move = move
	print("%s chose move %s" % [battler.name, move.name])

func choose_move(_state: BattleState) -> Move:
	return battler.moves.pick_random()
