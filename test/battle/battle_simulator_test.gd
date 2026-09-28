# GdUnit generated TestSuite
class_name BattleSimulatorTest
extends GdUnitTestSuite
@warning_ignore('unused_parameter')
@warning_ignore('return_value_discarded')

# TestSuite generated from
const __source: String = 'res://battle/battle_simulator.gd'

var simulator := BattleSimulator.new()

	
func _create_battler_controller(battler: Battler) -> BattlerController:
	var controller := BattlerController.new()
	controller.battler = battler
	return controller


func before() -> void:
	add_child(simulator)

func before_test() -> void:
	var player := _create_battler_controller(load("res://battle/battler/battlers/player_battler.tres"))
	var opponent := _create_battler_controller(load("res://battle/battler/battlers/opp_battler.tres"))
	
	simulator.battle = BattleState.new()
	simulator.battle.controllers = [player, opponent]

func test_is_player_turn() -> void:
	var player := simulator.player.battler
	var opponent := simulator.battle.battlers[1]
	
	# first turn, so it should be the players turn
	assert_object(player).is_same(simulator.battle.active_battler)
	
	# advance the simulation
	while simulator.turn_count == 0:
		simulator.step()
	
	# we should be on the second battler now
	assert_object(opponent).is_same(simulator.battle.active_battler)
