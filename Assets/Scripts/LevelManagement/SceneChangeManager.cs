using LevelGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : HappyTools.SingletonBehaviourFixed<SceneChangeManager>
{
    public bool IsChanging { get; private set; } = false;

    //TEMP : 게임 글로벌 데이터를 저장하는 곳으로 이동
    [SerializeField] LevelGraphData _levelGraphData;

    public PassageData GetNextPassageData(string passageName)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        var result = _levelGraphData.GetExitPassageData(new PassageData(sceneName, passageName));
        if (result.SceneName == "")
            return new PassageData(sceneName, passageName);
        return result;
    }
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
