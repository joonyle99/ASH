using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
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

    private IEnumerator Die()
    {
        yield return null;

        Debug.Log("Enter Die");
        Animator.SetBool("IsDead", true);
        this.GetComponent<Collider2D>().enabled = false;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        // 초기 알파값 저장
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            startAlphas[i] = renderers[i].color.a;
        }

        // 모든 렌더 컴포넌트를 돌면서 Fade Out
        float t = 0;
        while (t < 5)
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

        Debug.Log("Exit Die");
        Animator.SetBool("IsDead", false);

        // 오브젝트 삭제
        this.gameObject.SetActive(false);
        // Destroy(gameObject);

        yield return null;
    }
}

