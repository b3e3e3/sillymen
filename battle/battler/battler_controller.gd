extends Node
class_name BattlerController

@export var battler: Battler

var current_move: Move = null
var status_effects: Array[StatusEffect]

@onready var simulator: BattleSimulator = get_parent() # HACK


func _init(default_battler: Battler = null) -> void:
	if default_battler:
		battler = default_battler
		
func _ready() -> void:
	battler.initialize()
	
func remove_status_effect(e: StatusEffect) -> void:
	status_effects.erase(e)

func set_current_move(move: Move):
	current_move = move
	print("%s chose move %s" % [battler.name, move.name])

func choose_move(_state: BattleState) -> Move:
	return battler.moves.pick_random()
