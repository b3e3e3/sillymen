extends BattlerController
class_name PlayerController


func choose_move(_state: BattleState) -> Move:
	return await simulator.battle_box.move_selected
