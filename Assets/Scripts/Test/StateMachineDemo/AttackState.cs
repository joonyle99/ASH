using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMahineDemo
{

    public class AttackState : PlayerState
    {
        protected override void OnEnter()
        {
            Debug.Log("Start Attack");
            StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            Debug.Log("Attack with power " + Player.AttackPower);

            if (Player.PreviousState is WalkState)
                Debug.Log("Attack during walk");
            else
                Debug.Log("Attack during idle");

            yield return new WaitForSeconds(0.5f);
            ChangeState<IdleState>();
        }

        protected override void OnUpdate()
        {
            Debug.Log("Update Attack");
        }

        protected override void OnExit()
        {
            Debug.Log("Exit Attack");
        }
    }
}