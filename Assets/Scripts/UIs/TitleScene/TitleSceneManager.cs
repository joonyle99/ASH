using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.Instance.StartSceneChange("TestStage");
    }
    public void OnExitButtonClicked()
    {

    }
}
