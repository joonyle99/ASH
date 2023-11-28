using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private InputState _inputState;

    public GameObject flashLight;

    public bool isLightWorking = false;
    public float rotateSpeed = 30f;

    public float PlayerDir { get => this.transform.localScale.x; }

    void Update()
    {
        _inputState = InputManager.Instance.GetState();

        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            flashLight.SetActive(!flashLight.activeSelf);
            isLightWorking = flashLight.activeSelf;
        }

        // Light Source Up / Down Rotation
        if (isLightWorking)
        {
            flashLight.transform.Rotate(Vector3.forward, (PlayerDir > 0f ? rotateSpeed : -rotateSpeed) * _inputState.Vertical * Time.deltaTime);

            // 상한선 하한선 정하기

            // 1 사분면에 위치하도록
            if (flashLight.transform.localEulerAngles.z > 35f && flashLight.transform.localEulerAngles.z < 90f)
            {
                flashLight.transform.localEulerAngles = new Vector3(flashLight.transform.localEulerAngles.x,
                    flashLight.transform.localEulerAngles.y, 35f);
            }
            // 4 사분면에 위치하도록
            else if (flashLight.transform.localEulerAngles.z > 270f && flashLight.transform.localEulerAngles.z < 325f)
            {
                flashLight.transform.localEulerAngles = new Vector3(flashLight.transform.localEulerAngles.x,
                    flashLight.transform.localEulerAngles.y, 325f);
            }
        }
    }
}
