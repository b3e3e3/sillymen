extends Node
class_name BattleSimulator

const MAX_STATES := 100
const MSG_TIME := 1.0

@export var autorun: bool = false

@export_category("Components")
@export var controllers: Array[BattlerController]
@export var battle: BattleState

# TODO: decouple UI
@export_category("UI")
@export var animation_player: AnimationPlayer
@export var battle_box: BattleBox
@export var hp_boxes: Array[HPBox]
@export var sprites: Array[BattlerSprite]


#var previous_states: Array[BattleState] = []
var player: BattlerController
var turn_count: int = 0
var simulating := false

var _controller_by_battler: Dictionary = {}
var _sprite_by_battler: Dictionary = {}
var _hpbox_by_controller: Dictionary = {}


func _init(state: BattleState = null) -> void:
	if state:
		battle = state

func _ready() -> void:
	for s in sprites:
		_sprite_by_battler[s.battler] = s
	for b in hp_boxes:
		_hpbox_by_controller[b.controller] = b
	for c in controllers:
		_controller_by_battler[c.battler] = c
		if c is PlayerController or c == controllers[0]:
			player = c
		# register battlers
		if not battle.battlers.has(c.battler):
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
	
	if autorun:
		run_simulation()
	
func update_battle_box() -> void:
	if not battle_box: return
	var message := battle_box.get_message(battle)
	if message != "":
		battle_box.show_message(message)
	
#func log_current_state() -> void:
	#var prev_state := battle.duplicate(true)
	#previous_states.append(prev_state)
	#
	#if previous_states.size() > MAX_STATES:
		#previous_states.pop_front()
	
func get_controller_for(battler: Battler) -> BattlerController:
	return _controller_by_battler.get(battler)
	
func get_sprite_for(battler: Battler) -> BattlerSprite:
	return _sprite_by_battler.get(battler)
	
func get_hpbox_for(controller: BattlerController) -> HPBox:
	return _hpbox_by_controller.get(controller)
	
func _refresh_hpboxes() -> void:
	if hp_boxes.is_empty(): return
	for c in controllers:
		get_hpbox_for(c).update()

func _resolve_move(controller: BattlerController, move: Move = controller.current_move) -> Array[PlannedAction]:
	var actions: Array[PlannedAction] = []
	for result in move.get_results():
		actions.append_array(_resolve_move_result(controller, result))
	return actions
	
func _resolve_move_result(controller: BattlerController, result: MoveResult) -> Array[PlannedAction]:
	var actions: Array[PlannedAction] = []
	var valid_targets := result.get_valid_targets(
		controller.battler,
		battle.battlers
	)
	
	if not valid_targets.is_empty():
		var target_controller := get_controller_for(valid_targets.front())
		actions.append(PlannedAction.new(result, controller, target_controller))
	
	return actions


func _present_action(action: PlannedAction) -> void:
	var hits := 0
	
	for i in action.result.get_repeat_count():
		if action.target.battler.is_fainted(): break
		if not action.result.apply(action.user, action.target): break
		
		hits += 1
		_refresh_hpboxes()
		
		var spr := get_sprite_for(action.target.battler)
		if spr and action.result.has_animation():
			await spr.play_animation(action.result.get_animation())
			
		await show_message(action.result.get_hit_message())
	
	var msg := action.result.get_result_message() if hits > 0 else "But it missed!"
	await show_message(msg)
	
func show_message(text: String, duration: float = MSG_TIME) -> void:
	if battle_box == null or text == "": return
	
	battle_box.show_message(text)
	await get_tree().create_timer(duration).timeout

func is_player_turn() -> bool:
	return battle.active_battler == player.battler

func step() -> void:
	#log_current_state()
	
	battle.process_state()
	if battle_box:
		battle_box.update(battle)

		var message := battle_box.get_message(battle)
		if message != "":
			print(turn_count + 1, '. ', message)
			battle_box.show_message(message)
		
	var controller := get_controller_for(battle.active_battler)
	var spr := get_sprite_for(battle.active_battler)
	
	match battle.phase:
		BattleState.Phase.CHOICE:			
			@warning_ignore("redundant_await") # some controllers await choosing a move
			controller.set_current_move(await controller.choose_move(battle))
			
		BattleState.Phase.MOVE:
			# purge status effects
			for e in controller.status_effects.duplicate():
				if e.has_expired():
					controller.remove_status_effect(e)
					await show_message("%s no longer has %s!" % [controller.battler.name, e.name])
					continue
			
			if spr:
				await spr.play_animation(controller.current_move \
					.get_animation()) # TODO: missed move
			
			for action in _resolve_move(controller):
				if action.target.battler.is_fainted() and action.target != action.user:
					break
				await _present_action(action)
		
		BattleState.Phase.POST_MOVE:
			for e in controller.status_effects.duplicate():
				await _present_action(PlannedAction.new(e, controller, controller))
				
		BattleState.Phase.END:
			await show_message("Battle over.")
			var living_battlers := battle.get_living_battlers()
			
			if living_battlers.is_empty():
				await show_message("It was a draw!")
			elif player.battler in living_battlers:
				await show_message("You won!")
			else:
				await show_message("You lost...")
	print("phase=%s next=%s living=%d" % [
	BattleState.Phase.keys()[battle.phase],
	BattleState.Phase.keys()[battle._next_phase],
	battle.get_living_battlers().size()])

func run_simulation() -> void:
	simulating = true
	while simulating:
		await step()
		simulating = battle.phase != BattleState.Phase.END
