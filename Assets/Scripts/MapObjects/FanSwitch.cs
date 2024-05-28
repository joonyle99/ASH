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
        SceneEffectManager.Current.PushCutscene(new Cutscene(this, CoEpicMoment())); // 컷씬 시작
    }

    protected override void OnObjectInteractionExit()
    {
        this.IsInteractable = true;
    }

    private IEnumerator CoEpicMoment()
    {
        InputManager.Instance.ChangeInputSetter(_InputSetter);

        // 레버 위치 변경
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
            SceneEffectManager.Current.Camera.StartFollow(_windZoneArr[i].transform);   // 카메라 연출 => 바람장치 있는곳으로 카메라 이동
            yield return new WaitForSeconds(_cameraMoveDuration);
            // 켜지는 연출
            _windZoneArr[i].GetComponent<WindArea>().SetActive();   // 실제로 켜짐
            yield return new WaitForSeconds(1.0f);
        }
        InputManager.Instance.ChangeToDefaultSetter();

        ExitInteraction();  // 상호작용 종료
    }
    
    private IEnumerator CoTest()
    {
        Debug.Log("실행");
        

        yield return null;
    }
}
