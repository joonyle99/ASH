using System.Collections;
using UnityEngine;

public class Fire_FirePillar : Monster_IndependentSkill
{
    [SerializeField] private ParticleHelper _pillar;
    [SerializeField] private Collider2D _collider;

    public IEnumerator ExecutePillar()
    {
        _pillar.Play();

        yield return new WaitForSeconds(0.2f);
        _collider.enabled = true;
    }

    public override void OnDestroy()
    {
        StopAllCoroutines();
    }
}
