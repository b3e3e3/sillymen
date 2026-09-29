extends Resource
class_name Battler

@export_category("General")
@export var name: String
@export var level: int
@export var max_hp: int = 100

@export_category("Sprites")
@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@export_category("Moves")
@export var moves: Array[Move]

var _current_hp: int = -1
var current_hp: int:
	get:
		return _current_hp
	set(val):
		_current_hp = (clamp(val, 0, max_hp))

func initialize() -> void:
	_current_hp = max_hp

func is_fainted() -> bool:
	return current_hp == 0
