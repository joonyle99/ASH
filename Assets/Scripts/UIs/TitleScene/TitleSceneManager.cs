using System.Collections;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayCommonBGM("MainTheme");
    }

    public void OpenPrologueScene()
    {
        StartCoroutine(OpenPrologueSceneCoroutine());
    }
    private IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene("PrologueScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(SceneChangeCoroutine(sceneName));
    }
    private IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene(sceneName, () => SoundManager.Instance.PlayCommonBGM("Exploration1"));
    }
}
