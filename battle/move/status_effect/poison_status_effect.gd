extends StatusEffect
class_name PoisonStatusEffect

const DMG_MULT := 1.0 / 16

var _target_controller: BattlerController


func get_damage(target: BattlerController) -> int:
	return maxi(1, roundi(target.battler.max_hp * DMG_MULT))

func apply(controller: BattlerController, _target: BattlerController) -> bool:
	if controller.battler.current_hp == 0: return false
	controller.battler.current_hp -= get_damage(controller)
	
	_target_controller = controller
	count += 1
	return true

func get_result_message() -> String:
	return "%s took damage from %s!" % [_target_controller.battler.name, name]
