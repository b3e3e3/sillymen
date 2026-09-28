extends Node
class_name BattleSimulator

const MAX_STATES := 100
const MSG_TIME := 1.0

@export_category("Components")
@export var controllers: Array[BattlerController]
@export var battle: BattleState

# TODO: decouple UI
@export_category("UI")
@export var animation_player: AnimationPlayer
@export var battle_box: BattleBox
@export var hp_boxes: Array[HPBox]
@export var sprites: Array[BattlerSprite]


var player: BattlerController
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
		if c is PlayerController or c == controllers[0]:
			player = c
			print("Assumed player controller as %s" % [c.name])
		# register battlers
		battle.battlers.append(c.battler)
		print("Registered battler %s" % c.battler.name)
		
	battle.turn_completed.connect(func():
		turn_count += 1
	)
	
	if battle_box:
		battle_box.show_message("%s appeared!" % battle.battlers[1].name)
	
	if animation_player:
		animation_player.play(&"intro")
		await animation_player.animation_finished
		animation_player.play(&"RESET")
	
	run_simulation()
	
func update_battle_box() -> void:
	if not battle_box: return
	var message := battle_box.get_message(battle)
	if message != "":
		battle_box.show_message(message)
	
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
	
func _refresh_hpboxes() -> void:
	for c in controllers:
		get_hpbox_for(c).update()

func _resolve_move(controller: BattlerController) -> Array[PlannedAction]:
	var actions: Array[PlannedAction] = []
	for result in controller.current_move.get_results():		
		var valid_targets := result.get_valid_targets(
			controller.battler,
			battle.battlers
		)
		
		if valid_targets.is_empty(): continue
		var target_controller := get_controller_for(valid_targets.front())
		actions.append(PlannedAction.new(result, controller, target_controller))
		
	return actions

func _present_action(action: PlannedAction) -> void:
	var landed := true
	var msg := ""
	
	for i in action.result.get_repeat_count():
		if not action.result.apply(action.user, action.target):
			landed = false
			break
	
		_refresh_hpboxes()
	
		if action.result.has_animation():
			await get_sprite_for(action.target.battler)	\
					.play_animation(action.result.get_animation())
				
	if landed:
		msg = action.result.get_result_message()
	else:
		msg = "But it missed!"
	
	if battle_box and msg != "":
		battle_box.show_message(msg)
		await get_tree().create_timer(MSG_TIME).timeout

func step() -> void:
	log_current_state()
	
	battle.process_state()
	if battle_box:
		battle_box.update(battle)

		var message := battle_box.get_message(battle)
		if message != "":
			print(turn_count + 1, '. ', message)
			battle_box.show_message(message)
		
	match battle.phase:
		BattleState.Phase.CHOICE:
			var controller := get_controller_for(battle.active_battler)
			@warning_ignore("redundant_await")
			controller.set_current_move(await controller.choose_move(battle))
			
		BattleState.Phase.MOVE:
			var controller := get_controller_for(battle.active_battler)
			await get_sprite_for(battle.active_battler)					\
					.play_animation(controller.current_move				\
					.get_animation()) # TODO: missed move
			
			for action in _resolve_move(controller):
				await _present_action(action)
		
		BattleState.Phase.POST:
			pass

func run_simulation() -> void:
	while true:
		await step()
