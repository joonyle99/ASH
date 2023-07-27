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
    public void OnStartButtonClicked()
    {
        SceneManager.Instance.StartSceneChange("Stage1-1");
        //SceneManager.Instance.StartSceneChangeByPassage(_initialPassageData);
        //SoundManager.Instance.StopBGMFade(0.5f);
    }
    public void OnExitButtonClicked()
    {

    }

}
