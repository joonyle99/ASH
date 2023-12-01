using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateParticlesOnHit : MonoBehaviour, IAttackListener
{
    [SerializeField] ActivateParticleOnDestroy _particle;
    [SerializeField] float hitFromLeftAngle;
    [SerializeField] float hitFromRightAngle;
    public void OnHitted(bool isBasicAttack)
    {
        if (SceneContext.Current.Player.transform.position.x > transform.position.x)
            _particle.SetEmisionRotations(new Vector3(0, 0, hitFromRightAngle));
        else
            _particle.SetEmisionRotations(new Vector3(0, 0, hitFromLeftAngle));
    }
}
