extends Resource
class_name Battler

@export_category("General")
@export var name: String
@export var level: int
@export var max_hp: int = 100

@export_category("Sprites")
@export var primary_sprite: Texture2D
@export var secondary_sprite: Texture2D

@export_category("Moves")
@export var moves: Array[Move]
