@tool
extends Resource
class_name TypeChart

@export var types: Array[BattleType] = []


func _init() -> void:
	for k in BattleType.Type.keys():
		var type := BattleType.new()
		type.type = k
		types.append(type)

func get_multiplier(atk: BattleType.Type, def: BattleType.Type) -> float:
	for t in types:
		if t.type == atk:
			return t.effectiveness.get(def, 1.0)
	return 1.0

func get_multipliers(atk: BattleType.Type, defenders: Array[BattleType.Type]) -> float:
	var mult := 1.0
	for d in defenders:
		if d == BattleType.Type.NONE: continue
		print("%s eff against %s? %s" % [BattleType.Type.keys()[atk], BattleType.Type.keys()[d], get_multiplier(atk, d)])
		mult *= get_multiplier(atk, d)
	return mult
