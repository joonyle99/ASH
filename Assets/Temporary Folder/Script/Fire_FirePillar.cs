using System.Collections;
using UnityEngine;

public class Fire_FirePillar : Monster_IndependentSkill
{
    [SerializeField] private ParticleHelper _pillar;
    [SerializeField] private Collider2D _collider;

    public IEnumerator ExecutePillar()
    {
        _pillar.Play();

        _collider.enabled = true;

        var emissionTime = _pillar.GetEmissionLifeTime();
        yield return new WaitForSeconds(emissionTime / 4f * 3f);
        _collider.enabled = false;
    }

    public override void OnDestroy()
    {
        StopAllCoroutines();
    }
}
