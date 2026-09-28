extends RefCounted
class_name BattleResolution

var result: MoveResult
var target: BattlerController
var applied: bool

func _init(p_result: MoveResult, p_target: BattlerController, p_applied: bool) -> void:
	result = p_result
	target = p_target
	applied = p_applied
