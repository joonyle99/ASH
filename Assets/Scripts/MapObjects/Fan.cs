using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : InteractableObject
{
    [SerializeField] GameObject _windZone;
    [SerializeField] GameObject[] _windZoneArr;

    protected override void OnInteract()
    {
        // 카메라 연출 => 바람장치 있는곳으로 카메라 이동
        // 켜지는 연출
        _windZone.SetActive(!_windZone.activeSelf); // 실제로 켜짐
        // 1초 후
        // 카메라 돌아옴
        ExitInteraction();  // 상호작용 종료
    }
}
