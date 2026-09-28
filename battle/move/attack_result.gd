extends MoveResult
class_name AttackMoveResult

@export var damage: int


func apply(_controller: BattlerController, target: BattlerController) -> bool:
	if not can_apply(): return false
	if target.current_hp == 0: return false
	target.current_hp -= damage
	affected_targets.append(target)
	return true
