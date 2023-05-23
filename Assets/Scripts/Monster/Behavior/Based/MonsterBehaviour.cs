using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �ൿ�� ���������� �����ϴ� ��� Ŭ����
/// ����� : ���ؿ�
/// </summary>
public class MonsterBehaviour : StateMachineBase
{
    // ������ Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �ǰ� collision���� "PlayerBasicAttackHitbox" ������Ʈ�� ã��
        if (collision.GetComponent<PlayerBasicAttackHitbox>() != null)
        {
            Debug.Log("Hitted by basic attack");
        }
    }
}
