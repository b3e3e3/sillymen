# extends Resource
# class_name BattlerLegacy

# @export_category("General")
# @export var name: String
# @export var level: int = 1
# @export var max_hp: int = 100

# @export_category("Type")
# @export var primary_type: BattleType.Type = BattleType.Type.NORMAL
# @export var secondary_type: BattleType.Type = BattleType.Type.NONE

# @export_category("Sprites")
# @export var primary_sprite: Texture2D
# @export var secondary_sprite: Texture2D

# @export_category("Moves")
# @export var moves: Array[Move]

# @export_category("Stats")
# @export var attack: Stat = Stat.new(20.0)
# @export var defense: Stat = Stat.new(20.0)
# @export var critical_rate: Stat = Stat.new(1.0 / 8)


# var _current_hp: int = -1
# var current_hp: int:
# 	get:
# 		return _current_hp
# 	set(val):
# 		_current_hp = (clamp(val, 0, max_hp))

# func initialize() -> void:
# 	_current_hp = max_hp

# func is_fainted() -> bool:
# 	return current_hp == 0

# func get_types() -> Array[BattleType.Type]:
# 	return [primary_type, secondary_type]
	
