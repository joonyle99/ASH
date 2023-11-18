using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private InputState _inputState;

    public GameObject light;

    public bool isLightWorking = false;
    public float rotateSpeed = 30f;

    public float PlayerDir { get => this.transform.localScale.x; }

    void Update()
    {
        _inputState = InputManager.Instance.GetState();

        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            light.SetActive(!light.activeSelf);
            isLightWorking = light.activeSelf;
        }

        // Light Source Up / Down Rotation
        if (isLightWorking)
        {
            light.transform.Rotate(Vector3.forward, (PlayerDir > 0f ? rotateSpeed : -rotateSpeed) * _inputState.Vertical * Time.deltaTime);

            // ���Ѽ� ���Ѽ� ���ϱ�

            // 1 ��и鿡 ��ġ�ϵ���
            if (light.transform.localEulerAngles.z > 35f && light.transform.localEulerAngles.z < 90f)
            {
                light.transform.localEulerAngles = new Vector3(light.transform.localEulerAngles.x,
                    light.transform.localEulerAngles.y, 35f);
            }
            // 4 ��и鿡 ��ġ�ϵ���
            else if (light.transform.localEulerAngles.z > 270f && light.transform.localEulerAngles.z < 325f)
            {
                light.transform.localEulerAngles = new Vector3(light.transform.localEulerAngles.x,
                    light.transform.localEulerAngles.y, 325f);
            }
        }
    }
}
