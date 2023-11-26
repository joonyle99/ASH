using System.Collections;
using UnityEngine;

public class HealingState : PlayerState
{
    [Range(0f, 10f)] [SerializeField] float _healingTime;
    public ParticleSystem particleEffect;

    protected override void OnEnter()
    {
        Debug.Log("Enter Healing");

        Player.PlaySound_SE_Healing_01();

        StartCoroutine(Healing());
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

    /// <summary>
    /// �÷��̾� ġ�� �Լ�
    /// </summary>
    /// <returns></returns>
    IEnumerator Healing()
    {
        Animator.SetTrigger("Healing");
        Animator.SetBool("IsHealing", true);

        // ��ƼŬ ���� & ����
        ParticleSystem myEffect = Instantiate(particleEffect, transform.position, Quaternion.identity, transform);
        myEffect.Play();  // �ݺ��Ǵ� ����Ʈ

        yield return new WaitForSeconds(_healingTime);

        // ��ƼŬ ���� & �ı�
        myEffect.Stop();
        Destroy(myEffect.gameObject);

        /*
        Player.CurHp++;

        if (Player.CurHp > 10)
            Player.CurHp = 10;
        */

        Animator.SetBool("IsHealing", false);
        ChangeState<IdleState>();

        yield return null;
    }
}