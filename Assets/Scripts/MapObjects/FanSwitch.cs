using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSwitch : InteractableObject
{
    [SerializeField] GameObject[] _windZoneArr;
    [SerializeField] float _cameraMoveDuration;
    [SerializeField] InputSetterScriptableObject _InputSetter;

    protected override void OnInteract()
    {
        this.IsInteractable = false;
        SceneEffectManager.Current.PushCutscene(new Cutscene(this, CoEpicMoment())); // 컷씬 시작
    }

    protected override void OnInteractionExit()
    {
        this.IsInteractable = true;
    }

    private IEnumerator CoEpicMoment()
    {
        InputManager.Instance.ChangeInputSetter(_InputSetter);
        for (int i = 0; i < _windZoneArr.Length; i++)
        {
            SceneEffectManager.Current.Camera.StartFollow(_windZoneArr[i].transform);   // 카메라 연출 => 바람장치 있는곳으로 카메라 이동
            yield return new WaitForSeconds(_cameraMoveDuration);
            // 켜지는 연출
            _windZoneArr[i].SetActive(!_windZoneArr[i].activeSelf); // 실제로 켜짐
            yield return new WaitForSeconds(1.0f);
        }
        InputManager.Instance.ChangeToDefaultSetter();

        ExitInteraction();  // 상호작용 종료
    }
}
