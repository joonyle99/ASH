using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGround : MonoBehaviour
{

    [SerializeField] float _breakDelay = 0.5f;
    [SerializeField] SoundClipData _collisionSound;

    bool _isBreaking = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isBreaking)
            return;
            PlayerBehaviour player = collision.transform.GetComponent<PlayerBehaviour>();
        if (player != null && player.StateIs<DiveState>())
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
        SoundManager.Instance.PlaySFX(_collisionSound);
        Invoke("Break", _breakDelay);
        _isBreaking = true;
    }
    void Break()
    {
        Destroy(gameObject);
    }
}
