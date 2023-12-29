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
        // �ʱ� ����
        ChangeState<IdleState>();
        CurHp = _maxHp;
        RecentDir = 1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * RecentDir, transform.localScale.y, transform.localScale.z);

        // �ݶ��̴� Ȱ��ȭ
        this.GetComponent<Collider2D>().enabled = true;

        // ��ƼŬ ���� & ����
        ParticleSystem myEffect = Instantiate(respawnEffect, transform.position, Quaternion.identity, transform);
        myEffect.Play();  // �ݺ��Ǵ� ����Ʈ

        // �ڽ� ������Ʈ�� ��� ���� ������Ʈ�� �����´�
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(false);

        // �ʱ� ���İ� ����
        float[] startAlphas = new float[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            startAlphas[i] = renderers[i].color.a;

        // ��� ���� ������Ʈ�� ���鼭 Fade In
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

        // ��ƼŬ ���� & �ı�
        myEffect.Stop();
        Destroy(myEffect.gameObject);
        */

        yield return null;
    }
}