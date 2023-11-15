using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public GameObject light;
    public bool isWorking = false;
    public float playerDir = 1f;

    public float PlayerDir
    {
        get => playerDir;
        set => playerDir = this.transform.localScale.x;
    }

    private float rotateSpeed = 30f;

    void Update()
    {
        // Light Source ON / OFF
        if (Input.GetKeyDown(KeyCode.L))
        {
            light.SetActive(!light.activeSelf);
            isWorking = light.activeSelf;
        }

        // Light Source Up / Down Rotation
        if (isWorking)
        {
            // Q와 E로 Rotation을 조절한다.
            if (Input.GetKey(KeyCode.Q))
            {
                // 플레이어가 바라보는 방향에 따라 "위로" 회전
                light.transform.Rotate(Vector3.forward, (playerDir > 0f ? rotateSpeed : -rotateSpeed) * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                // 플레이어가 바라보는 방향에 따라 "아래로" 회전
                light.transform.Rotate(Vector3.forward, (playerDir > 0f ? (-1) * rotateSpeed : rotateSpeed) * Time.deltaTime);
            }
        }
    }
}
