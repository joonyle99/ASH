public abstract class MonsterState : StateBase
{
    public NewMonsterBehavior Monster { get { return StateMachine as NewMonsterBehavior; } }
}
