using System.Collections;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    //[SerializeField] PassageData _initialPassageData;
    [SerializeField]
    private OptionView _optionView;

    public GameObject HelperText;
    public GameObject Chapter1Cheat;
    public GameObject Chapter2Cheat;

    private void Update()
    {
        /*
        // HelperText
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.Space))
        {
            HelperText.SetActive(!HelperText.activeSelf);
        }
        */

        // CHEAT: F1 키를 누르면 첫 번째 챕터의 치트메뉴를 활성화한다
        if (Input.GetKeyDown(KeyCode.F1) && GameSceneManager.Instance.CheatMode == true)
        {
            Chapter1Cheat.SetActive(!Chapter1Cheat.activeSelf);
        }
        // CHEAT: F2 키를 누르면 두 번째 챕터의 치트메뉴를 활성화한다
        if (Input.GetKeyDown(KeyCode.F2) && GameSceneManager.Instance.CheatMode == true)
        {
            Chapter2Cheat.SetActive(!Chapter2Cheat.activeSelf);
        }
    }

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
        if (_optionView != null)
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
    /// 프롤로그 씬으로 전환합니다
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenPrologueSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToNonPlayableScene("NewPrologueScene");
    }
    /// <summary>
    /// 해당 씬으로 전환합니다
    /// </summary>
    /// <param name="sceneName">이동하려는 씬 이름</param>
    /// <returns></returns>
    IEnumerator SceneChangeCoroutine(string sceneName)
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToPlayableScene(sceneName, "Enter " + sceneName.ToString());
    }
}