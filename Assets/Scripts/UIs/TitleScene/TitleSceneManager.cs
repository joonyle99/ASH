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

    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene("PrologueScene");
    }
    IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene(sceneName, ()=>  SoundManager.Instance.PlayCommonBGM("Exploration1"));
    }
}
