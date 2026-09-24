extends Control
class_name Battle

enum State {
	WAIT,
	ACTIVE,
}

enum Phase {
	PRE,
	CHOICE,
	MOVE_CHOICE,
	RUNNING,
	POST,
}

@export var state := State.ACTIVE
@export var phase := Phase.PRE

@onready var box: Control = $BattleBox
@onready var moves_panel: Control = box.get_node(^"HBoxContainer/MovesPanel")
@onready var choice_panel: Control = box.get_node(^"HBoxContainer/ChoicePanel")
@onready var dialog_container: Control = box.get_node(^"HBoxContainer/DialogContainer")

var battlers: Array[Battler]


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	box.visible = false
	$AnimationPlayer.play(&"intro")
	
	for b in find_children("*", "Battler"):
		battlers.append(b as Battler)
	
	print("Found %d battler(s)!" % battlers.size())
	
func _process(delta) -> void:
	match phase:
		Phase.CHOICE:
			box.visible = true
			choice_panel.visible = true
			moves_panel.visible = false
			dialog_container.visible = true
		Phase.MOVE_CHOICE:
			box.visible = true
			choice_panel.visible = false
			moves_panel.visible = true
			dialog_container.visible = false
		Phase.RUNNING:
			box.visible = true
			choice_panel.visible = false
			moves_panel.visible = false
			dialog_container.visible = true
	

func _on_animation_player_animation_started(anim_name: StringName) -> void:
	state = State.WAIT

func build_moves_buttons(battler: Battler) -> Dictionary[Move, Button]:
	var buttons: Dictionary[Move, Button]
	var panel: Control = box.get_node(^"HBoxContainer/MovesPanel")
	
	var cont: Control = panel.get_node(^"MarginContainer/GridContainer")
	for c in cont.get_children():
		c.queue_free()
		
	for m in battler.moves:
		var button := Button.new()
		button.text = m.display_name
		button.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		button.size_flags_vertical = Control.SIZE_EXPAND_FILL
		cont.add_child.call_deferred(button)
		buttons[m] = button
	
	return buttons

func next_turn() -> void:
	var battler: Battler = battlers.pop_front()
	
	battler.take_turn()
	if battler.state == Battler.State.PLAYER_CHOICE:
		phase = Phase.CHOICE
	else:
		phase = Phase.RUNNING
	
	battlers.append(battler)

func _on_animation_player_animation_finished(anim_name: StringName) -> void:
	state = State.ACTIVE
	match anim_name:
		&"intro":
			next_turn()

func do_move(battler: Battler, move: Move) -> void:
	%Dialog.text = "%s used %s!" % [battler.name, move.display_name]
	battler.do_move(move)

func _on_attack_button_pressed() -> void:
	phase = Phase.MOVE_CHOICE
	
	var moves_to_buttons := build_moves_buttons($Main/PlayerContainer.get_child(0))
	for move in moves_to_buttons:
		var button := moves_to_buttons[move]
		button.pressed.connect(_on_move_button_pressed.bind(move), ConnectFlags.CONNECT_ONE_SHOT)

func _on_move_button_pressed(move: Move) -> void:
	phase = Phase.RUNNING
	var _pla: Battler = $Main/PlayerContainer.get_child(0)
	var _opp: Battler = $Main/OpponentContainer.get_child(0)
	
	_pla.current_move = move
	
	do_move(_pla, move)#_pla.moves[0])
	await _pla.player.animation_finished
	
	_opp.flash()
	await _opp.flash_finished
	
	next_turn()
	
	do_move(_opp, _opp.moves[0])
	await _opp.player.animation_finished
	
	_pla.flash()
	await _pla.flash_finished
	
	phase = Phase.CHOICE
	%Dialog.text = "What happens now?"

func _on_think_button_pressed() -> void:
	pass # Replace with function body.


func _on_try_button_pressed() -> void:
	pass # Replace with function body.


func _on_cower_button_pressed() -> void:
	pass # Replace with function body.
