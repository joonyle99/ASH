using System.Collections;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    //[SerializeField] PassageData _initialPassageData;

    private void Start()
    {
        SoundManager.Instance.PlayCommonBGM("MainTheme");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(SceneChangeCoroutine(sceneName));
    }

    public void OpenPrologueScene()
    {
        StartCoroutine(OpenPrologueSceneCoroutine());
    }

    /// <summary>
    /// 프롤로그 씬으로 전환합니다
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToNonPlayableScene("PrologueScene");
    }
    /// <summary>
    /// 해당 씬으로 전환합니다
    /// </summary>
    /// <param name="sceneName">이동하려는 씬 이름</param>
    /// <returns></returns>
    IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToNonPlayableScene(sceneName, ()=>  SoundManager.Instance.PlayCommonBGM("Exploration1"));
    }
}
