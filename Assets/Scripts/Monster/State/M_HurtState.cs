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
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // �����̴� ȿ���� �� �� �ݺ����� ����
        int numberOfTwinkles = 5;

        // �����̴� �� ����Ŭ�� ���� �ð�
        float twinkleDuration = 0.1f;

        for (int n = 0; n < numberOfTwinkles; n++)
        {
            // ��� �������� ���鼭 ������ ȿ���� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // ���� ������ �����ϰ� ����
                Color transparentColor = renderer.color;
                transparentColor.a = 0.5f; // �ణ �����ϰ� ����
                renderer.color = transparentColor;
            }

            // �� ���� �ð� ���� ���
            yield return new WaitForSeconds(twinkleDuration);

            // ��� �������� ���鼭 ���� �������� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // ���� ������ ������� ����
                Color originalColor = renderer.color;
                originalColor.a = 1f; // ���� �������ϰ� ����
                renderer.color = originalColor;
            }

            // �� ���� �ð� ���� ���
            yield return new WaitForSeconds(twinkleDuration);
        }

        // ȿ���� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ڵ带 �߰�
    }

    private IEnumerator TwinkleEffect2()
    {
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // �����̴� ȿ���� �� �� �ݺ����� ����
        int numberOfTwinkles = 5;
        // �����̴� �� ����Ŭ�� ���� �ð�
        float twinkleDuration = 0.1f;

        // ���� ���� ����
        Color[] originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;

        for (int t = 0; t < numberOfTwinkles; t++)
        {
            // ��� �������� ���鼭 ������� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                renderer.color = Color.white;
            }

            // �� ���� �ð� ���� ���
            yield return new WaitForSeconds(twinkleDuration);

            // ��� �������� ���鼭 ���� �������� ����
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = originalColors[i];
            }

            // �� ���� �ð� ���� ���
            yield return new WaitForSeconds(twinkleDuration);
        }

        // ȿ���� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ڵ带 �߰�
    }

    public void EndHurt_AnimEvent()
    {
        _isEnd = true;
    }
}