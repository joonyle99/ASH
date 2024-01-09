using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateParticlesOnHit : MonoBehaviour, IAttackListener
{
    [SerializeField] EnableParticlesOnDestruct _particle;
    [SerializeField] float hitFromLeftAngle;
    [SerializeField] float hitFromRightAngle;
    public void OnHitted(AttackInfo attackInfo)
    {
        if (SceneContext.Current.Player.transform.position.x > transform.position.x)
            _particle.AddEmissionRotations(hitFromRightAngle);
        else
            _particle.AddEmissionRotations(hitFromLeftAngle);
    }
}
