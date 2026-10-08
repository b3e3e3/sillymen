# extends Control
# class_name HPBox

# @onready var name_label: RichTextLabel = %NameLabel
# @onready var level_number_label: RichTextLabel = %LevelNumberLabel
# @onready var hp_number_label: RichTextLabel = %HPNumberLabel
# @onready var hp_bar: ProgressBar = %HPBar

# @export var controller: BattlerController


# func _init(p_controller: BattlerController = null) -> void:
# 	if p_controller:
# 		controller = p_controller

# func _ready() -> void:
# 	if not name_label:
# 		name_label = RichTextLabel.new()
# 		add_child(name_label)
	
# 	if not level_number_label:
# 		level_number_label = RichTextLabel.new()
# 		add_child(level_number_label)
		
# 	if not hp_number_label:
# 		hp_number_label = RichTextLabel.new()
# 		add_child(hp_number_label)
	
# 	if not hp_bar:
# 		hp_bar = ProgressBar.new()
# 		add_child(hp_bar)
	
# 	update()

# func update() -> void:
# 	name_label.text = controller.battler.name
# 	level_number_label.text = String.num_uint64(controller.battler.level)
# 	hp_number_label.text = String.num_uint64(controller.battler.current_hp)
# 	hp_bar.value = float(controller.battler.current_hp) / controller.battler.max_hp
