using System.Collections;
using UnityEngine;

public class M_HurtState : MonsterState
{
    private bool _isEnd;

    protected override void OnEnter()
    {
        Animator.SetTrigger("Hurt");

        _isEnd = false;
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
        // TODO
        yield return new WaitForSeconds(1f);
    }

    public void EndHurt_AnimEvent()
    {
        _isEnd = true;
    }
}