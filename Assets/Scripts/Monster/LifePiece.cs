using UnityEngine;

public class LifePiece : MonoBehaviour
{
    [SerializeField] private int _healingAmount;
    [SerializeField] private ParticleHelper[] _particleHelpers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 오직 플레이어와 상호작용한다

        // 플레이어의 CurHp를 증가시킨다
    }

}
