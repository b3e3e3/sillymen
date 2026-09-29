extends MoveResult
class_name AttackMoveResult

const DEBUG_CRIT_MULT := 2.0

@export var power: int
@export var min_hits: int = 1
@export var max_hits: int = 1

var _landed_hits := 0
var _last_hit_crit := false

func calculate_damage(controller: BattlerController, target: BattlerController) -> int:
	var dmg: int
	
	var crit := DEBUG_CRIT_MULT if _last_hit_crit else 1.0
	var level := controller.battler.level
	var attack := controller.battler.attack.value
	var defense := target.battler.defense.value
	var level_divisor := 32.0
	
	print("level=%s crit=%s power=%s atk=%s def=%s" % [level, crit, power, attack, defense])
	
	# DAMAGE ALGORITHM
	dmg = roundi(((((2.0 * level * crit) / 5.0) + 2.0) * power * (attack / defense)) / level_divisor + 2.0)
	
	var random := randf_range(0.85, 1.0) if dmg != 1 else 1.0
	
	dmg = ceili(dmg * random)
	
	return dmg

func get_repeat_count() -> int:
	return randi_range(min_hits, max_hits)

func apply(controller: BattlerController, target: BattlerController) -> bool:
	if not can_apply(): return false
	_last_hit_crit = randf() < controller.battler.critical_rate.value
	
	target.battler.current_hp -= calculate_damage(controller, target)
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
