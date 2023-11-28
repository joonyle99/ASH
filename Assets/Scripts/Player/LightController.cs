using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private GameObject _flashLight;

    [SerializeField] private bool _isUsableLightState;
    [SerializeField] private bool _isLightWorking;
    [SerializeField] private float _rotateSpeed = 30f;

    public float PlayerDir { get => this.transform.localScale.x; }

    void Update()
    {
        PlayerBehaviour playerBehaviour = this.GetComponent<PlayerBehaviour>();
        _isUsableLightState = playerBehaviour.StateIs<IdleState>() || playerBehaviour.StateIs<RunState>();

        if (this._isLightWorking)
        {
            if (!_isUsableLightState)
            {
                _flashLight.SetActive(false);
                _isLightWorking = _flashLight.activeSelf;
            }
        }

        InputState inputState = InputManager.Instance.GetState();

        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_isUsableLightState)
            {
                _flashLight.SetActive(!_flashLight.activeSelf);
                _isLightWorking = _flashLight.activeSelf;
            }
        }

        // Light Source Up / Down Rotation
        if (_isLightWorking)
        {
            _flashLight.transform.Rotate(Vector3.forward, (PlayerDir > 0f ? _rotateSpeed : -_rotateSpeed) * inputState.Vertical * Time.deltaTime);

            // 상한선 하한선 정하기

            // 1 사분면에 위치하도록
            if (_flashLight.transform.localEulerAngles.z > 35f && _flashLight.transform.localEulerAngles.z < 90f)
            {
                _flashLight.transform.localEulerAngles = new Vector3(_flashLight.transform.localEulerAngles.x,
                    _flashLight.transform.localEulerAngles.y, 35f);
            }
            // 4 사분면에 위치하도록
            else if (_flashLight.transform.localEulerAngles.z > 270f && _flashLight.transform.localEulerAngles.z < 325f)
            {
                _flashLight.transform.localEulerAngles = new Vector3(_flashLight.transform.localEulerAngles.x,
                    _flashLight.transform.localEulerAngles.y, 325f);
            }
        }
    }
}
