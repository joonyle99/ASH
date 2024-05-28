using System.Collections;
using UnityEngine;

public class FanSwitch : InteractableObject
{
    [SerializeField] GameObject[] _windZoneArr;
    [SerializeField] GameObject _lever;
    [SerializeField] float _cameraMoveDuration;
    [SerializeField] InputSetterScriptableObject _InputSetter;

    protected override void OnObjectInteractionEnter()
    {
        this.IsInteractable = false;
        SceneEffectManager.Current.PushCutscene(new Cutscene(this, CoEpicMoment())); // �ƾ� ����
    }

    protected override void OnObjectInteractionExit()
    {
        this.IsInteractable = true;
    }

    private IEnumerator CoEpicMoment()
    {
        InputManager.Instance.ChangeInputSetter(_InputSetter);

        // ���� ��ġ ����
        Vector3 angles = _lever.transform.rotation.eulerAngles;
        float t = 0f;

        while (t < 1.5f)
        {
            t += Time.deltaTime;
            float targetRot = -angles.z;
            float zRotation = Mathf.Lerp(angles.z, targetRot, t) % 360.0f;

            _lever.transform.eulerAngles = new Vector3(_lever.transform.eulerAngles.x, _lever.transform.eulerAngles.y, zRotation);
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < _windZoneArr.Length; i++)
        {
            SceneEffectManager.Current.Camera.StartFollow(_windZoneArr[i].transform);   // ī�޶� ���� => �ٶ���ġ �ִ°����� ī�޶� �̵�
            yield return new WaitForSeconds(_cameraMoveDuration);
            // ������ ����
            _windZoneArr[i].GetComponent<WindArea>().SetActive();   // ������ ����
            yield return new WaitForSeconds(1.0f);
        }
        InputManager.Instance.ChangeToDefaultSetter();

        ExitInteraction();  // ��ȣ�ۿ� ����
    }
    
    private IEnumerator CoTest()
    {
        Debug.Log("����");
        

        yield return null;
    }
}
