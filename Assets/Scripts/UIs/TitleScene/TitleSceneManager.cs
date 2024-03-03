using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayCommonBGM("MainTheme");
    }
    //[SerializeField] PassageData _initialPassageData;

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
    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene("PrologueScene");
    }
    IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene(sceneName, ()=>  SoundManager.Instance.PlayCommonBGM("Exploration1"));
    }
}
