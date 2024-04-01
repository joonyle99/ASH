using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : InteractableObject
{
    [SerializeField] GameObject _windZone;
    [SerializeField] GameObject[] _windZoneArr;

    protected override void OnInteract()
    {
        // ī�޶� ���� => �ٶ���ġ �ִ°����� ī�޶� �̵�
        // ������ ����
        _windZone.SetActive(!_windZone.activeSelf); // ������ ����
        // 1�� ��
        // ī�޶� ���ƿ�
        ExitInteraction();  // ��ȣ�ۿ� ����
    }
}
