using UnityEngine;

public class LifePiece : MonoBehaviour
{
    [SerializeField] private int _healingAmount;
    [SerializeField] private ParticleHelper[] _particleHelpers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� �÷��̾�� ��ȣ�ۿ��Ѵ�

        // �÷��̾��� CurHp�� ������Ų��
    }

}
