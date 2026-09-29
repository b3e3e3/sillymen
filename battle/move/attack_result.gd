extends MoveResult
class_name AttackMoveResult

const DEBUG_CRIT_MULT := 2.0

@export var damage: int
@export var min_hits: int = 1
@export var max_hits: int = 1

var _landed_hits := 0
var _last_hit_crit := false


func get_repeat_count() -> int:
	return randi_range(min_hits, max_hits)

func apply(controller: BattlerController, target: BattlerController) -> bool:
	if not can_apply(): return false
	_last_hit_crit = randf() < controller.battler.critical_rate
	
	target.battler.current_hp -= ceili(damage * (DEBUG_CRIT_MULT if _last_hit_crit else 1.0))
	affected_targets.append(target)
	_landed_hits += 1
	
	return true
	
func get_hit_message() -> String:
	if _last_hit_crit:
		return "Critical hit!"
	return ""

func get_result_message() -> String:
	if max_hits > 1:
		return "Hit %d time(s)!" % [_landed_hits]
	return ""
