using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashableRock : MonoBehaviour, IAttackListener
{
    [SerializeField] List<ParticleHelper> _particles;
    [SerializeField] List<Transform> _pieces;

    public void OnHitted(bool isBasicAttack)
    {
        SoundManager.Instance.PlayCommonSFXPitched("SE_CrashRock_hit");
        foreach (ParticleHelper particle in _particles)
        {
            if (SceneContext.Current.Player.transform.position.x > transform.position.x)
                particle.SetEmmisionRotation(new Vector3(0, 0, 60));
            else
                particle.SetEmmisionRotation(new Vector3(0, 0, 0));
            particle.Activate();
            particle.transform.parent = null;
            particle.transform.localScale = Vector3.one;
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
