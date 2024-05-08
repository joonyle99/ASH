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
        fireBall.Play();
    }

    public void AshPillarShift()
    {

    }

    public void FlamePillarExplosion()
    {

    }

    public IEnumerator StartSkill(float duration = 1f)
    {
        yield return new WaitForSeconds(duration);

        // FlameBeam();
        FireBallDrop();
    }
}
