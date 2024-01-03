using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneContextBuildListener
{
    /// <summary>
    /// Called between Awake and Start, when SceneContext is usable
    /// </summary>
    public void OnSceneContextBuilt();
}
