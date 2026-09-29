extends Node
class_name BattlerController

@export var battler: Battler

var current_move: Move = null
var _current_hp: int
var current_hp: int:
	get:
		return _current_hp
	set(val):
		_current_hp = (clamp(val, 0, battler.max_hp))

@onready var simulator: BattleSimulator = get_parent() # HACK


func _init(default_battler: Battler = null) -> void:
	if default_battler:
		battler = default_battler

func _ready() -> void:
	current_hp = battler.max_hp

func set_current_move(move: Move):
	current_move = move
	print("%s chose move %s" % [battler.name, move.name])

func choose_move(_state: BattleState) -> Move:
	return battler.moves.pick_random()
