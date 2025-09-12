public interface ISceneContextBuildListener
{
    public int Priority => 10;    // 우선순위 (낮을수록 먼저 호출됨)
    /// <summary>
    /// Called When SceneContext is usable
    /// </summary>
    public void OnSceneContextBuilt();
}
