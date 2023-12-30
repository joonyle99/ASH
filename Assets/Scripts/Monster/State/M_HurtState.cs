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
        // �ڽ� ������Ʈ�� ��� SpriteRenderer�� �����´�
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        for (int n = 0; n < _countOfTwinkles; n++)
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
            yield return new WaitForSeconds(_twinkleDuration);

            // ��� �������� ���鼭 ���� �������� ����
            foreach (SpriteRenderer renderer in spriteRenderers)
            {
                // ���� ������ ������� ����
                Color originalColor = renderer.color;
                originalColor.a = 1f; // ���� �������ϰ� ����
                renderer.color = originalColor;
            }

            // �� ���� �ð� ���� ���
            yield return new WaitForSeconds(_twinkleDuration);
        }

        // ȿ���� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ڵ带 �߰�
    }

    public void EndHurt_AnimEvent()
    {
        _isEnd = true;
    }
}