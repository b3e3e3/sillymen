extends MoveResult
class_name AttackMoveResult

@export var damage: int
@export var min_hits: int = 1
@export var max_hits: int = 1

var _landed_hits := 0


func get_repeat_count() -> int:
	return randi_range(min_hits, max_hits)

func apply(_controller: BattlerController, target: BattlerController) -> bool:
	if not can_apply(): return false
	if target.current_hp == 0: return false
	
	target.current_hp -= damage
	affected_targets.append(target)
	_landed_hits += 1
	
	return true

func get_result_message() -> String:
	if _landed_hits > 1:
		return "Hit %d time(s)!" % [_landed_hits]
	return ""
