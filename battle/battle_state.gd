# extends Resource
# class_name BattleStateLegacy

# signal turn_started(battler: Battler)
# signal turn_completed

# enum Phase {
# 	START,
# 	CHOICE,
# 	MOVE,
# 	POST_MOVE,
# 	END,
# }

# @export_storage var battlers: Array[Battler]
# @export_storage var active_battler: Battler
# var phase := Phase.START
# var _next_phase := phase


# func get_living_battlers() -> Array[Battler]:
# 	var _battlers: Array[Battler]
# 	_battlers.assign(battlers.filter(func(e): return not e.is_fainted()))
# 	return _battlers

# func next_battler() -> void:
# 	battlers.push_back(battlers.pop_front())

# func process_state() -> void:
# 	phase = _next_phase
	
# 	if phase == Phase.CHOICE and get_living_battlers().size() <= 1:
# 		phase = Phase.END
# 		_next_phase = Phase.END
	
# 	match phase:
# 		Phase.START:
# 			_next_phase = Phase.CHOICE
# 		Phase.CHOICE:
# 			while battlers.front().is_fainted():
# 				next_battler()
# 			active_battler = battlers.front()
# 			turn_started.emit(active_battler)
# 			_next_phase = Phase.MOVE
# 		Phase.MOVE:
# 			_next_phase = Phase.POST_MOVE
# 		Phase.POST_MOVE:
# 			next_battler()
# 			turn_completed.emit()
# 			_next_phase = Phase.CHOICE
# 		Phase.END:
# 			pass
