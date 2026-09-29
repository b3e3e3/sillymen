extends Resource
class_name BattleType

enum Type {
	NORMAL,
	CRAZY,
	SCARY,
	SILLY,
	NONE,
}

@export var type: Type
@export var effectiveness: Dictionary[Type, float] = {}
