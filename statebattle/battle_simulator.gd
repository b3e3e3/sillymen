extends Node
class_name BattleSimulator

@export var battle: BattleState
@export var player: BattlerController
@export var battle_box: BattleBox

var controllers: Array[BattlerController]

var previous_states: Array[BattleState] = []
var turn_count: int = 0
var is_simulating := false


func is_player_turn() -> bool:
	return battle.active_battler == player.battler

func _ready() -> void:
	for c in find_children("*", "BattlerController"):
		controllers.append(c as BattlerController)
	
	# register battlers
	for c in controllers:
		battle.battlers.append(c.battler)
		print("Registered %s" % c.battler.name)
		
	battle.turn_completed.connect(func():
		turn_count += 1
	)
	
	run_simulation()
	
func log_current_state() -> void:
	var prev_state := battle.duplicate(true)
	previous_states.append(prev_state)
	
func get_controller_for(battler: Battler) -> BattlerController:
	var idx := controllers.find_custom(func(c):
		return c.battler == battler
	)
	return controllers[idx]

func run_simulation() -> void:
	# simulation
	while true:	
		log_current_state()
		
		battle.process_state()
		battle_box.update(battle)

		var message := battle_box.get_message(battle)
		if message != "":
			print(turn_count + 1, '. ', message)
			
		match battle.phase:
			BattleState.Phase.CHOICE:
				var controller := get_controller_for(battle.active_battler)
				@warning_ignore("redundant_await")
				controller.set_current_move(await controller.choose_move(battle))
				
			BattleState.Phase.MOVE:
				await get_tree().create_timer(1).timeout
			
			BattleState.Phase.POST:
				pass
