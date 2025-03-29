namespace HappyTools
{
    public abstract class GameBootstrapper : SingletonBehaviourFixed<GameBootstrapper>
    {
        /// <summary>
        /// Initializes the settings.
        /// </summary>
        public abstract void InitSetting();
        /// <summary>
        /// Initializes the game.
        /// </summary>
        public abstract void InitGame();
    }
}
