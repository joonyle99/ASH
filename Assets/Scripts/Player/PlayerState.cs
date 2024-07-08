public abstract class PlayerState : StateBase
{
    public PlayerBehaviour Player { get => StateMachine as PlayerBehaviour; }
}