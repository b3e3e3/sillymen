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

var _controller_by_battler: Dictionary = {}
var _sprite_by_battler: Dictionary = {}
var _hpbox_by_controller: Dictionary = {}


func is_player_turn() -> bool:
	return battle.active_battler == player.battler

func _ready() -> void:
	assert(player in controllers, "Player must be one of controllers")
	
	for s in sprites:
		_sprite_by_battler[s.battler] = s
	for b in hp_boxes:
		_hpbox_by_controller[b.controller] = b
	for c in controllers:
		_controller_by_battler[c.battler] = c
		
		# register battlers
		battle.battlers.append(c.battler)
		print("Registered battler %s" % c.battler.name)
		
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
	return _controller_by_battler[battler]
	
func get_sprite_for(battler: Battler) -> BattlerSprite:
	return _sprite_by_battler[battler]
	
func get_hpbox_for(controller: BattlerController) -> HPBox:
	return _hpbox_by_controller[controller]

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
				
				## HACK
				#var valid_targets := 								\
						#controller.current_move						\
						#.get_valid_targets(battle.active_battler,	\
						#battle.battlers)
				#var target: Battler = valid_targets.front()
				
				#var results := controller.resolve_move(get_controller_for(target))
				var results := controller.current_move.get_results()
					
				for result in results:
					# HACK
					var valid_targets := 													\
							result															\
							.get_valid_targets(controller.current_move.target_resolver, 	\
								battle.active_battler,										\
							battle.battlers)
					var target: Battler = valid_targets.front()
					if not result.apply(controller, get_controller_for(target)): continue
					
					battle_box.update(battle)
					
					for c in controllers:
						get_hpbox_for(c).update()
					
					if result.has_animation():
						await get_sprite_for(target)	\
								.play_animation(result	\
								.get_animation())
				
			
			BattleState.Phase.POST:
				pass
