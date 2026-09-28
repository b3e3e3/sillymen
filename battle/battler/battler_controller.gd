extends Node
class_name BattlerController

@export var battler: Battler

var current_move: Move = null
var current_hp: int

@onready var simulator: BattleSimulator = get_parent() # HACK


func _ready() -> void:
	current_hp = battler.max_hp
	
func resolve_move(target: BattlerController) -> MoveResult:
	current_move.result.apply(self, target)
	return current_move.result

func set_current_move(move: Move):
	current_move = move
	print("%s chose move %s" % [battler.name, move.name])

func choose_move(_state: BattleState) -> Move:
	return battler.moves.pick_random()
