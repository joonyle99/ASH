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

    public void OpenPrologueScene()
    {
        StartCoroutine(OpenPrologueSceneCoroutine());
    }
    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene("PrologueScene");
    }
}
