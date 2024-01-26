using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{

    [SerializeField] float _breakDelay = 0.5f;

    bool _isBreaking = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isBreaking)
            return;

        PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null && player.CurrentStateIs<DiveState>())
        {
            StartBreaking();
            return;
        }

        RollingStone stone = collision.transform.GetComponent<RollingStone>();
        if (stone != null && stone.IsBreakable)
        {
            StartBreaking();
            return;
        }    

    }
    void StartBreaking()
    {
        Invoke("Break", _breakDelay);
        _isBreaking = true;
    }
    void Break()
    {
        Destruction.Destruct(gameObject);
    }
}
