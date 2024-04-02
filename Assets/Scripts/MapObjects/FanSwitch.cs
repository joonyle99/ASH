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
        SceneEffectManager.Current.PushCutscene(new Cutscene(this, CoEpicMoment())); // �ƾ� ����
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
            SceneEffectManager.Current.Camera.StartFollow(_windZoneArr[i].transform);   // ī�޶� ���� => �ٶ���ġ �ִ°����� ī�޶� �̵�
            yield return new WaitForSeconds(_cameraMoveDuration);
            // ������ ����
            _windZoneArr[i].SetActive(!_windZoneArr[i].activeSelf); // ������ ����
            yield return new WaitForSeconds(1.0f);
        }
        InputManager.Instance.ChangeToDefaultSetter();

        ExitInteraction();  // ��ȣ�ۿ� ����
    }
}
