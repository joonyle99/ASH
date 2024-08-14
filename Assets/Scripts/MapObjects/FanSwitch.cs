using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FanSwitch : InteractableObject, ISceneContextBuildListener
{
    [SerializeField] GameObject[] _windZoneArr;
    [SerializeField] GameObject _lever;
    [SerializeField] float _cameraMoveDuration;
    [SerializeField] InputSetterScriptableObject _InputSetter;

    PreserveState _statePreserver;

    [SerializeField] private float _leverAngleZ = 50f;

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();
    }

    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            float newLeverAngleZ = _leverAngleZ;

            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                newLeverAngleZ = _statePreserver.LoadState<float>("_leverAngleZSaved", newLeverAngleZ);
            }
            else
            {
                newLeverAngleZ = _statePreserver.LoadState<float>("_leverAngleZ", newLeverAngleZ);
            }

            _lever.transform.eulerAngles = new Vector3(_lever.transform.eulerAngles.x, _lever.transform.eulerAngles.y, newLeverAngleZ);
        }

        SaveAndLoader.OnSaveStarted += SaveFanSwitchState;
    }

    protected override void OnDestroy()
    {
        if(_statePreserver)
        {
            _statePreserver.SaveState<float>("_leverAngleZ", _leverAngleZ);
        }

        SaveAndLoader.OnSaveStarted -= SaveFanSwitchState;
    }

    protected override void OnObjectInteractionEnter()
    {
        this.IsInteractable = false;
        SceneEffectManager.Instance.PushCutscene(new Cutscene(this, CoEpicMoment())); // 컷씬 시작
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

        _leverAngleZ = _lever.transform.eulerAngles.z;

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < _windZoneArr.Length; i++)
        {
            SceneEffectManager.Instance.Camera.StartFollow(_windZoneArr[i].transform);   // 카메라 연출 => 바람장치 있는곳으로 카메라 이동
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

    private void SaveFanSwitchState()
    {
        if(_statePreserver)
        {
            _statePreserver.SaveState<float>("_leverAngleZSaved", _leverAngleZ);
        }
    }
}
