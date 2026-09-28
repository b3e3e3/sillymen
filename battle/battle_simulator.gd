extends Node
class_name BattleSimulator

signal move_finished

const MAX_STATES := 100

# TODO: decouple UI
@export var animation_player: AnimationPlayer
@export var battle: BattleState
@export var player: BattlerController
@export var battle_box: BattleBox
@export var hp_boxes: Array[HPBox]
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
	
	# HACK
	animation_player.play(&"intro")
	battle_box.update(battle)
	await animation_player.animation_finished
	animation_player.play(&"RESET")
	
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
	
func get_hp_box_for(controller: BattlerController) -> HPBox:
	var idx := hp_boxes.find_custom(func(b):
		return b.controller == controller
	)
	assert(idx != -1, "No HPBox found for %s" % [controller.name])
	return hp_boxes[idx]

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
				var controller := get_controller_for(battle.active_battler)
				await get_sprite_for(battle.active_battler)					\
						.play_animation(controller.current_move				\
						.get_animation())
				
				# HACK
				var valid_targets := 								\
						controller.current_move						\
						.get_valid_targets(battle.active_battler,	\
						battle.battlers)
				var target: Battler = valid_targets.front()
				
				var results := controller.resolve_move(get_controller_for(target))
				
				# HACK -- get_hp_box_for might be slow, dictionary maybe?
				for c in controllers:
					get_hp_box_for(c).update()
					
				for result in results:
					if result.has_animation():
						await get_sprite_for(target)	\
								.play_animation(result	\
								.get_animation())
				
				battle_box.update(battle)
			
			BattleState.Phase.POST:
				pass
