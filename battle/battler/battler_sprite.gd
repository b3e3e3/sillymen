@tool
extends Control
class_name BattlerSprite

signal _move_resolved
signal hit_frame

@export var battler: Battler

@export_category("Sprites")
@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@onready var texture: TextureRect = $TextureRect
@onready var animation_player: AnimationPlayer = $AnimationPlayer


func _ready() -> void:
	switch_primary_sprite()
		
func trigger_hit_frame() -> void:
	hit_frame.emit()

func play_animation(anim: StringName) -> void:
	if not animation_player.has_animation(anim):
		push_warning("No animation '%s' found" % [anim])
		return
	
	animation_player.play(anim)
	await animation_player.animation_finished
	animation_player.play(&"RESET")
	await animation_player.animation_finished
	
	# WIP: hit frames, but theyre currently unused. works tho
	#var resolved := [false] # HACK: array so lambda captures
	#var on_hit := func():
		#if resolved[0]: return
		#resolved[0] = true
		#_move_resolved.emit()
	#
	#var on_finished := func(_anim):
		#if resolved[0]: return
		#resolved[0] = true
		#_move_resolved.emit()
		#
	#hit_frame.connect(on_hit)
	#animation_player.animation_finished.connect(on_finished)
	#
	#animation_player.play(anim)
	#
	#await _move_resolved
	#
	#hit_frame.disconnect(on_hit)
	#animation_player.animation_finished.disconnect(on_finished)
	
func switch_primary_sprite() -> void:
	texture.texture = primary_sprite
	
func switch_secondary_sprite() -> void:
	texture.texture = secondary_sprite
