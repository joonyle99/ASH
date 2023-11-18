using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashableStone : MonoBehaviour, IAttackListener
{
    [SerializeField] List<ParticleHelper> _particles;
    [SerializeField] List<Transform> _soundPieces;

    public void OnHitted(bool isBasicAttack)
    {
        SoundManager.Instance.PlayCommonSFXPitched("SE_CrashRock_hit");
        foreach (ParticleHelper particle in _particles)
        {
            if (SceneContext.Current.Player.transform.position.x > transform.position.x)
                particle.SetEmisionRotation(new Vector3(0, 0, 60));
            else
                particle.SetEmisionRotation(new Vector3(0, 0, 0));
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

        Destroy(transform.gameObject);
    }
}
