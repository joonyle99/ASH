using System.Collections;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    //[SerializeField] PassageData _initialPassageData;
    [SerializeField]
    private OptionView _optionView;

    public GameObject Chapter1Cheat;
    public GameObject Chapter2Cheat;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Chapter1Cheat.SetActive(!Chapter1Cheat.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Chapter2Cheat.SetActive(!Chapter2Cheat.activeSelf);
        }
    }
#endif

    public void OpenPrologueScene()
    {
        StartCoroutine(OpenPrologueSceneCoroutine());
    }
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(SceneChangeCoroutine(sceneName));
    }
    public void LoadSavedScene()
    {
        PersistentDataManager.LoadToSavedData();
    }
    public void ToggleSettingPanel()
    {
        if(_optionView != null)
        {
            _optionView.TogglePanel();
        }
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, "Enter " + sceneName.ToString());
    }
}