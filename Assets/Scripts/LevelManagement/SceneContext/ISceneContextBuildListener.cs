public interface ISceneContextBuildListener
{
    /// <summary>
    /// Called between Awake and Start, When SceneContext is usable
    /// </summary>
    public void OnSceneContextBuilt();
}
