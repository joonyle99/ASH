using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] float _breakDelay = 0.5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null && player.StateIs<DiveState>())
        {
            Invoke("Break", _breakDelay);
            return;
        }

        RollingStone stone = collision.transform.GetComponent<RollingStone>();
        if (stone != null && stone.IsBreakable)
        {
            Invoke("Break", _breakDelay);
            return;
        }    

    }
    void Break()
    {
        Destroy(gameObject);
    }
}
