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
    /// 플레이어 치유 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator Healing()
    {
        Animator.SetTrigger("Healing");
        Animator.SetBool("IsHealing", true);

        // 파티클 생성 & 시작
        ParticleSystem myEffect = Instantiate(particleEffect, transform.position, Quaternion.identity, transform);
        myEffect.Play();  // 반복되는 이펙트

        yield return new WaitForSeconds(_healingTime);

        // 파티클 종료 & 파괴
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