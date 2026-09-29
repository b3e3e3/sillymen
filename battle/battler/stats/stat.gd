extends Resource
class_name Stat

@export var base: float
@export var min_value: float = 0.0
@export var max_value: float = 255.0
@export var min_stage: int = -4
@export var max_stage: int = 4

var _cached_value: float
var _dirty := true

var stage: int = 0:
	set(val):
		stage = clampi(val, min_stage, max_stage)
		_dirty = true

var value: float:
	get:
		if _dirty:
			_cached_value = _calculate()
			_dirty = false
		return _cached_value


func _init(stat_value: float = 20.0) -> void:
	base = stat_value

func _calculate() -> float:
	var flat := 0.0
	var mult := 1.0
	# TODO: for m in modifiers...
	var staged := (base + flat) * get_stage_multiplier() * mult
	return clampf(staged, min_value, max_value)
	
func get_stage_multiplier() -> float:
	# Pokemon-style: +1 = 3/2, +2 = 4/2, -1 = 2/3, ...
	return (2.0 + maxi(stage, 0)) / (2.0 + maxi(-stage, 0))
	
func add_modifier(_m) -> void:
	# TODO
	_dirty = true
	
func remove_modifier(_m) -> void:
	# TODO
	_dirty = true
	
func change_stage(by: int) -> void:
	stage += by
