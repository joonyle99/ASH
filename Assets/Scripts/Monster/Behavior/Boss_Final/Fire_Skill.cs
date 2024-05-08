using System.Collections;
using UnityEngine;

public class Fire_Skill : MonoBehaviour
{
    [Header("FlameBeam")]
    [Space]

    public Transform beamSpawnPoint;
    public int beamCount = 8;
    public FlameLaser flameBeam;

    [Header("FireBall")]
    [Space]

    public ParticleSystem fireBall;

    [Header("AshPillar")]
    [Space]

    public AshPillar ashPillar;

    public void FlameBeam()
    {
        var eachBeamAngle = 360f / beamCount;

        for (int i = 0; i < beamCount; i++)
        {
            var angle = i * eachBeamAngle;
            var beam = Instantiate(flameBeam, beamSpawnPoint.position, Quaternion.Euler(0f, 0f, angle), beamSpawnPoint);
            beam.GetComponent<Animator>().SetTrigger("Dissolve");
        }
    }

    public void FireBallDrop()
    {
        var fireBallObject = Instantiate(fireBall, transform.position, Quaternion.identity);

        var velocityModule = fireBallObject.velocityOverLifetime;
        
        // direction
        var vec1 = new Vector2(1f, -1f).normalized;
        var vec2 = new Vector2(-1f, -1f).normalized;
        var vec3 = new Vector2(0f, -1f).normalized;

        velocityModule.x = new ParticleSystem.MinMaxCurve(vec1.x, vec1.x);
        velocityModule.y = new ParticleSystem.MinMaxCurve(vec1.y, vec1.y);

        fireBallObject.Play();
    }

    public void AshPillarShift()
    {
        var dir = Random.Range(0, 2) == 0 ? 1 : -1;
        var pos = transform.position + Vector3.right * (-1) * dir * 10f;
        var ash = Instantiate(ashPillar, pos, Quaternion.identity);
        ash.SetDirection(dir);
    }

    public void FlamePillarExplosion()
    {

    }

    public IEnumerator StartSkill(float duration = 1f)
    {
        yield return new WaitForSeconds(duration);

        // FlameBeam();
        // FireBallDrop();
        AshPillarShift();
    }
}
