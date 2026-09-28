extends Resource
class_name TargetResolver

enum TargetType {
	SELF,
	OTHER,
}


func resolve(target_type: TargetType, user: Battler, battlers: Array[Battler]) -> Array[Battler]:
	match target_type:
		TargetType.SELF:
			return battlers.filter(func(b): return user == b)
		TargetType.OTHER:
			return battlers.filter(func(b): return user != b)
	return []
