using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashableRock : AttackableEntity
{
    [SerializeField] ParticleHelper _particle;
    [SerializeField] List<Transform> _pieces;
    override protected void OnHittedByBasicAttack(PlayerBehaviour player)
    {
        if (player.transform.position.x > transform.position.x)
            _particle.SetEmmisionRotation(new Vector3(0, 0, 60));
        else
            _particle.SetEmmisionRotation(new Vector3(0, 0, 0));
        _particle.Activate();
        _particle.transform.parent = transform.parent;

        GetComponent<Collider2D>().enabled = false;

        foreach (var piece in _pieces)
        {
            piece.transform.parent = transform.parent;
            piece.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

}
