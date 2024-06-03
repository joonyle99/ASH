namespace HappyTools
{
    public abstract class GameBootstrapper : SingletonBehaviourFixed<GameBootstrapper>
    {
        /// <summary>
        /// This is called right before the scene loads, after all Awake() calls and before all Start().
        /// </summary>
        public abstract void InitGame();
    }
}
