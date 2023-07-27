using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    public Transform respawnPoint;
    [Range(0f, 10f)] public float disapearTime;
    [Range(0f, 10f)] public float reviveDelay;

    protected override void OnEnter()
    {
        StartCoroutine(Die());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    /// <summary>
    /// 플레이어가 죽는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator Die()
    {
        Animator.SetBool("IsDead", true);

        // 콜라이더 비활성화
        this.GetComponent<Collider2D>().enabled = false;

        // 자식 오브젝트의 모든 렌더 컴포넌트를 가져온다
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        // 초기 알파값 저장
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        float t = 0;
        while (t < disapearTime)
        {
            t += Time.deltaTime;
            float normalizedTime = t / 2;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 0f, normalizedTime);
                renderers[i].color = color;
            }

            yield return null;
        }

        Animator.SetBool("IsDead", false);

        // 오브젝트의 사망 처리
        gameObject.SetActive(false);
        SceneManager.Instance.ReactivatePlayerAfterDelay(respawnPoint.position, reviveDelay);

        yield return null;
    }
}

