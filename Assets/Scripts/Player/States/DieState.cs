using System.Collections;
using UnityEngine;

public class DieState : PlayerState
{
    protected override void OnEnter()
    {
        Animator.SetBool("IsDead", true);
        StartCoroutine(Die());
    }

    protected override void OnUpdate()
    {

    }

    protected override void OnExit()
    {
        Animator.SetBool("IsDead", false);
    }

    private IEnumerator Die()
    {
        this.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(2f);

        this.gameObject.SetActive(false);

        yield return null;
    }
}

