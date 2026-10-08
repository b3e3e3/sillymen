namespace Sillymen;

public readonly partial struct PlannedAction(
    MoveResult Result,
    BattlerController User,
    BattlerController Target)
{

    public MoveResult Result { get; init; } = Result;
    public BattlerController User { get; init; } = User;
    public BattlerController Target { get; init; } = Target;

    public bool ApplyMoveResult() => Result.apply(User, Target);
}