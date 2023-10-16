using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashableStone : AttackableEntity
{
    [SerializeField] List<ParticleHelper> _particles;
    [SerializeField] List<Transform> _soundPieces;
    override protected void OnHittedByBasicAttack(PlayerBehaviour player)
    {
        SoundManager.Instance.PlayCommonSFXPitched("SE_CrashRock_hit");
        foreach (ParticleHelper particle in _particles)
        {
            if (player.transform.position.x > transform.position.x)
                particle.SetEmmisionRotation(new Vector3(0, 0, 60));
            else
                particle.SetEmmisionRotation(new Vector3(0, 0, 0));
            particle.Activate();
            particle.transform.parent = null;
            particle.transform.localScale = Vector3.one;
            particle.transform.rotation = Quaternion.identity;
        }
        foreach (var piece in _soundPieces)
        {
            piece.transform.parent = null;
            piece.gameObject.SetActive(true);
        }

        Destroy(transform.parent.gameObject);
    }


}
