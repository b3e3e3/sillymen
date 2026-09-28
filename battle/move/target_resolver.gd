extends Resource
class_name TargetResolver


func get_self_targets(user: Battler, battlers: Array[Battler]) -> Array[Battler]:
	return battlers.filter(func(b):
		return user == b
	)
	
func get_other_targets(user: Battler, battlers: Array[Battler]) -> Array[Battler]:
	return battlers.filter(func(b):
		return user != b
	)
