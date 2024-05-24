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
    /// ���ѷα� ������ ��ȯ�մϴ�
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToNonPlayableScene("PrologueScene");
    }
    /// <summary>
    /// �ش� ������ ��ȯ�մϴ�
    /// </summary>
    /// <param name="sceneName">�̵��Ϸ��� �� �̸�</param>
    /// <returns></returns>
    IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToNonPlayableScene(sceneName, ()=>  SoundManager.Instance.PlayCommonBGM("Exploration1"));
    }
}
