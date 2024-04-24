namespace HappyTools
{
    public abstract class GameBootstrapper : SingletonBehaviourFixed<GameBootstrapper>
    {
        /// <summary>
        /// This is called right before the scene loads, before all Awake() calls.
        /// </summary>
        public abstract void InitGame();
    }
}
