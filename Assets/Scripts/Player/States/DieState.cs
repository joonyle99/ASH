using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    protected override void OnEnter()
    {
        Animator.SetTrigger("Die");

        Player.IsDead = true;

        // StartCoroutine(Alive());

        // Player.InstantRespawn();
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnFixedUpdate()
    {

    }

    protected override void OnExit()
    {

    }

    private IEnumerator Alive()
    {
        Debug.Log("Alive");

        /*
        // 초기 설정
        ChangeState<IdleState>();
        CurHp = _maxHp;
        RecentDir = 1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

        // 콜라이더 활성화
        this.GetComponent<Collider2D>().enabled = true;

        // 파티클 생성 & 시작
        ParticleSystem myEffect = Instantiate(respawnEffect, transform.position, Quaternion.identity, transform);
        myEffect.Play();  // 반복되는 이펙트

        // 자식 오브젝트의 모든 렌더 컴포넌트를 가져온다
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(false);

        // 초기 알파값 저장
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;

        // 모든 렌더 컴포넌트를 돌면서 Fade In
        float t = 0;
        while (t < _reviveFadeInDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / _reviveFadeInDuration;

            for (int i = 0; i < renderers.Length; i++)
            {
                Color color = renderers[i].color;
                color.a = Mathf.Lerp(startAlphas[i], 1f, normalizedTime);
                renderers[i].color = color;
                CapeRenderer.sharedMaterial.SetFloat("_Opacity", normalizedTime);
            }

            yield return null;
        }

        // 파티클 종료 & 파괴
        myEffect.Stop();
        Destroy(myEffect.gameObject);
        */

        yield return null;
    }
}