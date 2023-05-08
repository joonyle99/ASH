using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : StateMachineBase
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerBasicAttackHitbox>() != null)
        {
            Debug.Log("Hitted by basic attack");
        }
    }
}
