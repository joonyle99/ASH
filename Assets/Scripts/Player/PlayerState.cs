public abstract class PlayerState : StateBase
{
    public PlayerBehaviour Player { get { return StateMachine as PlayerBehaviour; } }
}