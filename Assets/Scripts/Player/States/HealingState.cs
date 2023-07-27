using System.Collections;
using UnityEngine;

public class HealingState : PlayerState
{
    [Range(0f, 10f)] [SerializeField] float _healingTime;

    protected override void OnEnter()
    {
        Debug.Log("Enter Healing");

        StartCoroutine(Healing());
    }

    protected override void OnUpdate()
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

        yield return new WaitForSeconds(_healingTime);

        Animator.SetBool("IsHealing", false);
        ChangeState<IdleState>();

        yield return null;
    }
}