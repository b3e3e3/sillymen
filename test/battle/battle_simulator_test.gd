# GdUnit generated TestSuite
class_name BattleSimulatorTest
extends GdUnitTestSuite
@warning_ignore('unused_parameter')
@warning_ignore('return_value_discarded')

# TestSuite generated from
const __source: String = 'res://battle/battle_simulator.gd'

var simulator: BattleSimulator

var player_controller: BattlerController
var player_sprite: BattlerSprite
var player_hpbox: HPBox

var opp_controller: BattlerController
var opp_sprite: BattlerSprite
var opp_hpbox: HPBox


func _create_simulator() -> BattleSimulator:
	return BattleSimulator.new(BattleState.new())
	
func _create_controller(battler: Battler) -> BattlerController:
	var c := BattlerController.new(battler)
	simulator.add_child(c)
	simulator.controllers.append(c)
	return c
	
func _create_sprites() -> void:
	player_sprite = BattlerSprite.new(player_controller.battler)
	simulator.add_child(player_sprite)
	simulator.sprites.append(player_sprite)
	
	opp_sprite = BattlerSprite.new(opp_controller.battler)
	simulator.add_child(opp_sprite)
	simulator.sprites.append(opp_sprite)
	
func _create_hpboxes() -> void:
	player_hpbox = HPBox.new(player_controller)
	add_child(player_hpbox)
	simulator.hp_boxes.append(player_hpbox)
	
	opp_hpbox = HPBox.new(opp_controller)
	add_child(opp_hpbox)
	simulator.hp_boxes.append(opp_hpbox)

func before() -> void:
	simulator = _create_simulator()
	
	player_controller = _create_controller(load("res://battle/battler/battlers/player_battler.tres"))
	opp_controller = _create_controller(load("res://battle/battler/battlers/opp_battler.tres"))
	
	_create_sprites()
	_create_hpboxes()
	
	add_child(simulator)

func test_is_player_turn() -> void:
	var player_battler := simulator.player.battler
	var is_player_turn := simulator.is_player_turn()
	
	assert_bool(is_player_turn).is_equal(player_battler == simulator.battle.active_battler)
	
	# advance the simulation
	var tc := simulator.turn_count
	while simulator.turn_count == tc:
		simulator.step()
	
	# we should be on the next battler now
	assert_bool(is_player_turn == simulator.is_player_turn()).is_false()

func test_get_controller_for() -> void:
	assert_object(simulator.get_controller_for(simulator.player.battler)).is_equal(player_controller)
	
func test_get_sprite_for() -> void:
	assert_object(simulator.get_sprite_for(simulator.player.battler)).is_equal(player_sprite)
	
func test_get_hpbox_for() -> void:
	assert_object(simulator.get_hpbox_for(simulator.player)).is_equal(player_hpbox)

func test_refresh_hpboxes() -> void:
	var cont := simulator.controllers[0]
	var old_name: String = cont.battler.name
	cont.battler.name = "Fuck Head"
	
	var box := simulator.get_hpbox_for(cont)
	simulator._refresh_hpboxes()
	
	assert_str(box.name_label.text).is_equal(cont.battler.name)
	
	# revert to old name
	cont.battler.name = old_name
