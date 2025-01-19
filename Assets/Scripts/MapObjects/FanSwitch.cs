using System.Collections;
using UnityEngine;

public class FanSwitch : InteractableObject, ISceneContextBuildListener
{
    [SerializeField] private InputSetterScriptableObject _InputSetter;

    [Space]

    [SerializeField] private GameObject _lever;
    [SerializeField] private GameObject[] _windZoneArr;

    [Space]

    [SerializeField] private float _leverRotateDuration = 0.4f;
    [SerializeField] private float _cameraMoveDelay = 1.0f;
    [SerializeField] private float _cameraMoveDuration = 1.5f;

    // 저장
    private float _leverAngleZ = 50f;
    private PreserveState _statePreserver;

    private void Awake()
    {
        _statePreserver = GetComponent<PreserveState>();
    }

    public void OnSceneContextBuilt()
    {
        if (_statePreserver)
        {
            if (SceneChangeManager.Instance.SceneChangeType == SceneChangeType.Loading)
            {
                float newLeverAngleZ = _statePreserver.LoadState<float>("_leverAngleZSaved", _leverAngleZ);
                _lever.transform.eulerAngles = new Vector3(_lever.transform.eulerAngles.x, _lever.transform.eulerAngles.y, newLeverAngleZ);
            }
            else
            {
                float newLeverAngleZ = _statePreserver.LoadState<float>("_leverAngleZ", _leverAngleZ);
                _lever.transform.eulerAngles = new Vector3(_lever.transform.eulerAngles.x, _lever.transform.eulerAngles.y, newLeverAngleZ);
            }
        }

        _leverAngleZ = _lever.transform.eulerAngles.z;

        SaveAndLoader.OnSaveStarted -= SaveFanSwitchState;
        SaveAndLoader.OnSaveStarted += SaveFanSwitchState;
    }

    protected override void OnDestroy()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState<float>("_leverAngleZ", _leverAngleZ);
        }

        SaveAndLoader.OnSaveStarted -= SaveFanSwitchState;
    }

    protected override void OnObjectInteractionEnter()
    {
        this.IsInteractable = false;

        SoundManager.Instance.PlayCommonSFX("SE_Lever_Work");

        StartCoroutine(SceneEffectManager.Instance.PushCutscene(new Cutscene(this, SwitchCoroutine())));
    }
    protected override void OnObjectInteractionExit()
    {
        this.IsInteractable = true;
    }

    private IEnumerator SwitchCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_InputSetter);

        Vector3 currentRotation = _lever.transform.eulerAngles;

        float startRotationZ = currentRotation.z;
        float targetRotationZ = (-1) * currentRotation.z;

        // 1. 각도 정규화 (0 ~ 360도 사이로)
        if (startRotationZ < 0) startRotationZ += 360f;
        else if (startRotationZ > 360f) startRotationZ -= 360f;
        if (targetRotationZ < 0) targetRotationZ += 360f;
        else if (targetRotationZ > 360f) targetRotationZ -= 360f;

        // 2. 각도 차이 계산 (-180도 ~ 180도 사이로)
        float deltaRotationZ = targetRotationZ - startRotationZ;
        if (deltaRotationZ < -180f) deltaRotationZ += 360f;
        else if (deltaRotationZ > 180f) deltaRotationZ -= 360f;

        // 3. 목표 각도 설정
        targetRotationZ = startRotationZ + deltaRotationZ;

        // Debug.Log($"<color=orange>startRotationZ</color>: {startRotationZ} / <color=yellow>targetRotationZ</color>: {targetRotationZ}");

        float eTime = 0f;

        // 레버 회전
        while (eTime < _leverRotateDuration)
        {
            float t = joonyle99.Math.EaseOutQuad(eTime / _leverRotateDuration);
            float nextRotationZ = Mathf.Lerp(startRotationZ, targetRotationZ, t);

            // Debug.Log($"<color=green>nextRotationZ</color>: {nextRotationZ}");

            _lever.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, nextRotationZ);

            yield return null;

            eTime += Time.deltaTime;
        }

        _lever.transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, targetRotationZ);

        // 레버 각도 저장
        _leverAngleZ = _lever.transform.eulerAngles.z;

        yield return new WaitForSeconds(_cameraMoveDelay);

        for (int i = 0; i < _windZoneArr.Length; i++)
        {
            SceneEffectManager.Instance.Camera.StartFollow(_windZoneArr[i].transform);
            yield return new WaitForSeconds(_cameraMoveDuration);
            _windZoneArr[i].GetComponent<WindArea>().SetActive();
            yield return new WaitForSeconds(_cameraMoveDuration);
        }

        InputManager.Instance.ChangeToDefaultSetter();

        ExitInteraction();  // 상호작용 종료
    }

    private void SaveFanSwitchState()
    {
        if (_statePreserver)
        {
            _statePreserver.SaveState<float>("_leverAngleZSaved", _leverAngleZ);
        }
    }
}
