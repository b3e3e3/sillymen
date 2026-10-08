# extends MoveResult
# class_name StatusMoveResult

# @export var status_effect: StatusEffect

# var _applied_to: BattlerController

# func get_result_message() -> String:
# 	if _applied_to:
# 		return "%s contracted %s!" % [_applied_to.battler.name, status_effect.name]
# 	return ""
	
# func apply(_controller: BattlerController, target: BattlerController) -> bool:
# 	if not can_apply(): return false
# 	target.status_effects.append(status_effect.duplicate())
# 	_applied_to = target
	
# 	return true


# #extends StatusEffect
# #class_name PoisonStatusEffect
# #
# #const DAMAGE_MULT := 1.0 / 8
# #
# #
# #func get_damage(target: BattlerController) -> int:
# 	#return maxi(1, floori(target.battler.current_hp * DAMAGE_MULT))
# #
# #func _on_applied(target: BattlerController) -> void:
# 	#target.battler.current_hp -= get_damage(target)
