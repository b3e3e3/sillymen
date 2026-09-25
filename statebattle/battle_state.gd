extends Resource
class_name BattleState

signal turn_started(battler: Battler)
signal turn_completed

enum Phase {
	START,
	CHOICE,
	MOVE,
	POST,
}

#@export var phase: Phase:
	#get:
		#return _phase
	#set(val):
		#_phase = val
		#phase_changed.emit(val)

@export var battlers: Array[Battler]

#var _phase := Phase.CHOICE
var phase := Phase.START
var _next_phase := phase
var active_battler: Battler
#var message: String = "No state."


func process_state() -> void:
	phase = _next_phase
	
	match phase:
		Phase.START:
			_next_phase = Phase.CHOICE
		Phase.CHOICE:
			active_battler = battlers.front()
			
			turn_started.emit(active_battler)
			
			_next_phase = Phase.MOVE
		Phase.MOVE:
			active_battler.take_turn(self)
			
			_next_phase = Phase.POST
			
		Phase.POST:
			battlers.push_back(battlers.pop_front())
			
			turn_completed.emit()
			
			_next_phase = Phase.CHOICE
