using System.Collections;
using UnityEngine;

public class M_HurtState : MonsterState
{
    [SerializeField] private int _countOfTwinkles = 5;
    [SerializeField] private float _twinkleDuration = 0.1f;

    private bool _isEnd;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");

        _isEnd = false;

        StartCoroutine(TwinkleEffect());
    }

    protected override void OnUpdate()
    {
        if (_isEnd)
        {
            _isEnd = false;
            ChangeState<M_IdleState>();
            return;
        }
    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {
        _isEnd = false;
    }

    private IEnumerator TwinkleEffect()
    {
        // 자식 오브젝트의 모든 SpriteRenderer를 가져온다
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        for (int n = 0; n < _countOfTwinkles; n++)
        {
            // 모든 렌더러를 돌면서 깜빡임 효과를 적용
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 투명하게 설정
                Color transparentColor = renderer.color;
                transparentColor.a = 0.5f; // 약간 투명하게 설정
                renderer.color = transparentColor;
            }

            // 반 지속 시간 동안 대기
            yield return new WaitForSeconds(_twinkleDuration);

            // 모든 렌더러를 돌면서 원래 색상으로 복구
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 원래대로 설정
                Color originalColor = renderer.color;
                originalColor.a = 1f; // 완전 불투명하게 설정
                renderer.color = originalColor;
            }

            // 반 지속 시간 동안 대기
            yield return new WaitForSeconds(_twinkleDuration);
        }

        // 효과가 끝난 후 추가 동작이 필요하면 여기에 코드를 추가
    }

    public void EndHurt_AnimEvent()
    {
        _isEnd = true;
    }
}