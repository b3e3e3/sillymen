extends Resource
class_name Battler

signal turn_taken

@export_category("General")
@export var name: String

@export_category("Sprites")
@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@export_category("Moves")
@export var moves: Array[Move]

var current_move: Move = null


#func take_turn(state: BattleState) -> void:
	##current_move = moves.pick_random()
	#turn_taken.emit()
	#current_move.do(self)
	
func resolve_move(target: Battler) -> MoveResult:
	return current_move.get_result(self, target)
