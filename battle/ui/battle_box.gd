extends Control
class_name BattleBox

enum Screen {
	NORMAL,
	MOVE_CHOICE,
}

signal move_selected(move: Move)

@export var simulator: BattleSimulator

@export var dialog_container: Control
@export var dialog_label: RichTextLabel
@export var choice_container: Control
@export var moves_container: Control

var message: String = ""
var screen := Screen.NORMAL


func build_move_choices(battler: Battler) -> void:
	var scene := preload("res://battle/ui/move_button.tscn")
	
	for c in moves_container.get_children():
		c.queue_free()
	
	for move in battler.moves:
		var button := scene.instantiate() as Button
		button.text = move.name
		button.pressed.connect(_on_move_button_pressed.bind(move), ConnectFlags.CONNECT_ONE_SHOT)
		moves_container.add_child.call_deferred(button)

func show_message(text: String) -> void:
	message = text
	update(simulator.battle)

func get_message(state: BattleState) -> String:
	match state.phase:
		BattleState.Phase.CHOICE:
			if simulator.is_player_turn():
				return "What will %s do?" % [state.active_battler.name]
			#else:
				#return "%s is picking a move..." % [state.active_battler.name]
		BattleState.Phase.MOVE:
			print("%s is about to use a move" % [state.active_battler.name])
			var controller := simulator.get_controller_for(state.active_battler)
			return "%s used %s!" % [state.active_battler.name, controller.current_move.name]
		#BattleState.Phase.POST_MOVE:
	return ""

func update(state: BattleState) -> void:	
	dialog_label.text = message#get_message(state)
	match state.phase:
		BattleState.Phase.CHOICE:
			match screen:
				Screen.NORMAL:
					dialog_container.visible = true
					if simulator.is_player_turn():
						choice_container.visible = true
					else:
						choice_container.visible = false
					moves_container.visible = false
				Screen.MOVE_CHOICE:
					dialog_container.visible = false
					choice_container.visible = false
					moves_container.visible = true
		BattleState.Phase.MOVE, BattleState.Phase.POST_MOVE:
			dialog_container.visible = true
			choice_container.visible = false
			moves_container.visible = false
		_:
			dialog_container.visible = true
			choice_container.visible = false
			moves_container.visible = false

func _on_move_button_pressed(move: Move) -> void:
	move_selected.emit(move)
	screen = Screen.NORMAL

func _on_attack_button_pressed() -> void:
	build_move_choices(simulator.player.battler)
	screen = Screen.MOVE_CHOICE
	update(simulator.battle)
