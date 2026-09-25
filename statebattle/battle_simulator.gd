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

func simulate() -> void:
	# simulation
	var prev_state := battle.duplicate(true)
	previous_states.append(prev_state)

	battle.process_state()
	battle_box.update(battle)

	var message := battle_box.get_message(battle)
	if message != "":
		print(turn_count + 1, '. ', message)
		#await get_tree().create_timer(1).timeout

func step() -> void:
	if is_simulating: return
	is_simulating = true
	simulate()
	if should_wait():
		await get_tree().create_timer(1).timeout
	is_simulating = false

func is_player_choice() -> bool:
	return is_player_turn() and battle.phase == BattleState.Phase.CHOICE

func should_simulate() -> bool:
	return not is_player_turn() or not is_player_choice()
	
func should_wait() -> bool:
	return battle.phase == BattleState.Phase.MOVE

func _process(_delta: float) -> void:
	if is_simulating: return
	if should_simulate():
		step()
	

func _on_attack_button_pressed() -> void:
	pass

func _on_move_selected(move: Move) -> void:
	if not is_player_turn(): return
	player.set_current_move(move)
	step()
