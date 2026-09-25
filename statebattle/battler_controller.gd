extends Node
class_name BattlerController

@export var battler: Battler
var simulator: BattleSimulator = get_parent() # HACK


func set_current_move(move: Move):
	battler.current_move = move
	print("%s chose move %s" % [battler.name, move.name])

func choose_move(_state: BattleState) -> Move:
	return battler.moves.pick_random()
