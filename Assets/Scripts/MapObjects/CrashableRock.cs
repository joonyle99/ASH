using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashableRock : AttackableEntity
{
    [SerializeField] List<ParticleHelper> _particles;
    [SerializeField] List<Transform> _pieces;
    override protected void OnHittedByBasicAttack(PlayerBehaviour player)
    {
        foreach(ParticleHelper particle in _particles)
        {
            if (player.transform.position.x > transform.position.x)
                particle.SetEmmisionRotation(new Vector3(0, 0, 60));
            else
                particle.SetEmmisionRotation(new Vector3(0, 0, 0));
            particle.Activate();
            particle.transform.parent = transform.parent;
        }

        GetComponent<Collider2D>().enabled = false;

        foreach (var piece in _pieces)
        {
            piece.transform.parent = transform.parent;
            piece.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

}
