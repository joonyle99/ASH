using UnityEngine;

public class Bootstrapper : HappyTools.GameBootstrapper
{
    public override void InitSetting()
    {
        // QualitySettings.vSyncCount = 1;
        // Application.targetFrameRate = 60; // 최대 프레임을 60FPS로 제한
        Debug.unityLogger.logEnabled = Application.isEditor; // 디버그 로그 비활성화
    }
    public override void InitGame()
    {
        InputManager.Instance.ChangeToDefaultSetter();
        //SaveDataManager.Instance.Init();
    }
}
