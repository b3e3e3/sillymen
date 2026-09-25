extends Node
class_name BattleSimulator

const MAX_STATES := 100

@export var battle: BattleState
@export var player: BattlerController
@export var battle_box: BattleBox
@export var sprites: Array[BattlerSprite]
@export var controllers: Array[BattlerController]

var previous_states: Array[BattleState] = []
var turn_count: int = 0


func is_player_turn() -> bool:
	return battle.active_battler == player.battler

func _ready() -> void:
	assert(player in controllers, "Player must be one of controllers")
		
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
	
	if previous_states.size() > MAX_STATES:
		previous_states.pop_front()
	
func get_controller_for(battler: Battler) -> BattlerController:
	var idx := controllers.find_custom(func(c):
		return c.battler == battler
	)
	assert(idx != -1, "No BattlerController found for %s" % [battler.name])
	return controllers[idx]
	
func get_sprite_for(battler: Battler) -> BattlerSprite:
	var idx := sprites.find_custom(func(s):
		return s.battler == battler
	)
	assert(idx != -1, "No BattlerSprite found for %s" % [battler.name])
	return sprites[idx]

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
				var sprite := get_sprite_for(battle.active_battler)
				await sprite.play_animation(battle.active_battler.current_move)
				battle.active_battler.resolve_move()
				battle_box.update(battle)
			
			BattleState.Phase.POST:
				pass
