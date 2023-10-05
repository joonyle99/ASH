using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : HappyTools.SingletonBehaviourFixed<SceneChangeManager>
{
    public bool IsChanging { get; private set; } = false;

    //TEMP for game start
    private void Start()
    {
        SceneContext sceneContext = FindFirstObjectByType<SceneContext>();
        Result buildResult = sceneContext.BuildPlayable("");
    }
    public void ChangeToPlayableScene(string sceneName, string passageName)
    {
        if (IsChanging)
            return;
        StartCoroutine(ChangeToPlayableSceneCoroutine(sceneName, passageName));
    }
    public void ChangeToScene(string sceneName)
    {

    }

    IEnumerator ChangeToPlayableSceneCoroutine(string sceneName, string passageName)
    {
        IsChanging = true;

        AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => load.isDone);

        SceneContext sceneContext= FindFirstObjectByType<SceneContext>();
        Result buildResult = sceneContext.BuildPlayable(passageName);
        IsChanging = false;
    }

}
