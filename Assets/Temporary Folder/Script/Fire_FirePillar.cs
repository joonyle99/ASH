using System.Collections;
using UnityEngine;

public class Fire_FirePillar : Monster_IndependentSkill
{
    [SerializeField] private ParticleHelper _pillar;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private float _colliderDelayTime = 0.2f;

    public IEnumerator ExecutePillar()
    {
        _pillar.Play();

        yield return new WaitForSeconds(_colliderDelayTime);
        _collider.enabled = true;
    }

    public override void OnDestroy()
    {
        StopAllCoroutines();
    }
}
