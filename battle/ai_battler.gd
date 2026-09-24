@tool
extends Battler
class_name AIBattler


func take_turn() -> void:
	current_move = moves.pick_random()
	state = State.MOVE
