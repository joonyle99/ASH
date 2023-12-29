using System.Collections;
using UnityEngine;

public class M_HurtState : MonsterState
{
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
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 깜빡이는 효과를 몇 번 반복할지 결정
        int numberOfTwinkles = 5;

        // 깜빡이는 한 사이클의 지속 시간
        float twinkleDuration = 0.1f;

        for (int n = 0; n < numberOfTwinkles; n++)
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
            yield return new WaitForSeconds(twinkleDuration);

            // 모든 렌더러를 돌면서 원래 색상으로 복구
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // 현재 색상을 원래대로 설정
                Color originalColor = renderer.color;
                originalColor.a = 1f; // 완전 불투명하게 설정
                renderer.color = originalColor;
            }

            // 반 지속 시간 동안 대기
            yield return new WaitForSeconds(twinkleDuration);
        }

        // 효과가 끝난 후 추가 동작이 필요하면 여기에 코드를 추가
    }

    private IEnumerator TwinkleEffect2()
    {
        // 자식 오브젝트의 모든 SpriteRenderer를 가져온다
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // 깜빡이는 효과를 몇 번 반복할지 결정
        int numberOfTwinkles = 5;
        // 깜빡이는 한 사이클의 지속 시간
        float twinkleDuration = 0.1f;

        // 원래 색상 저장
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;

        for (int t = 0; t < numberOfTwinkles; t++)
        {
            // 모든 렌더러를 돌면서 흰색으로 변경
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                renderer.color = Color.white;
            }

            // 반 지속 시간 동안 대기
            yield return new WaitForSeconds(twinkleDuration);

            // 모든 렌더러를 돌면서 원래 색상으로 복구
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = originalColors[i];
            }

            // 반 지속 시간 동안 대기
            yield return new WaitForSeconds(twinkleDuration);
        }

        // 효과가 끝난 후 추가 동작이 필요하면 여기에 코드를 추가
    }

    public void EndHurt_AnimEvent()
    {
        _isEnd = true;
    }
}