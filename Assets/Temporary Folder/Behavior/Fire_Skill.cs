using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fire_Skill : MonoBehaviour
{
    [Header("FlameBeam")]
    [Space]

    public Fire_FlameBeam flameBeam;
    public Transform flameBeamSpawnPoint;
    public int beamCount = 8;

    [Header("FireBall")]
    [Space]

    public Fire_FireBall fireBall;
    public Transform fireBallSpawnPoint;
    public int fireBallCount;
    public int fireBallCastCount;
    public float fireBallCastInterval;

    [Header("AshPillar")]
    [Space]

    public Fire_AshPillar ashPillar;
    public float ashPillarSpawnDistance;
    public int ashPillarCastCount;
    public float ashPillarCastInterval;

    [Header("FirePillar")]
    [Space]

    public Fire_FirePillar firePillar;
    public Range firePillarSpawnRange;
    public float firePillarSpawnHeight;
    public int firePillarCount;
    public float firePillarEachDistance;

    private List<float> _usedPosX;

    public void FlameBeam()
    {
        var eachBeamAngle = 360f / beamCount;

        for (int i = 0; i < beamCount; i++)
        {
            var angle = i * eachBeamAngle; // 0 ~ 360
            var flameBeamInstance = Instantiate(flameBeam, flameBeamSpawnPoint.position, Quaternion.Euler(0f, 0f, angle));
            flameBeamInstance.GetComponent<Animator>().SetTrigger("Dissolve");
        }
    }
    public void FireBall()
    {
        StartCoroutine(FireBallCoroutine());
    }
    private IEnumerator FireBallCoroutine()
    {
        for (int i = 0; i < fireBallCastCount; i++)
        {
            var fireBallInstance = Instantiate(fireBall, fireBallSpawnPoint.position, Quaternion.identity);

            var fireBallParticle = fireBallInstance.GetComponent<ParticleSystem>();
            var velocityModule = fireBallParticle.velocityOverLifetime;
            var emissionModule = fireBallParticle.emission;

            // velocity module
            var dir1 = new Vector2(1f, -1f).normalized;                             // 북서 -> 남동
            var dir2 = new Vector2(-1f, -1f).normalized;                            // 북동 -> 남서
            var dir3 = new Vector2(0f, -1f).normalized;                             // 북 -> 남

            var targetDir = GetRandomDirection(dir1, dir2, dir3);         // '가변 개수 인수' 사용

            velocityModule.x = new ParticleSystem.MinMaxCurve(targetDir.x);
            velocityModule.y = new ParticleSystem.MinMaxCurve(targetDir.y);

            // emission module
            var burst = emissionModule.GetBurst(0);
            burst.count = new ParticleSystem.MinMaxCurve(fireBallCount);

            emissionModule.SetBurst(0, burst);

            // play particle
            fireBallParticle.Play();

            // cast interval
            yield return new WaitForSeconds(fireBallCastInterval);
        }
    }
    private Vector2 GetRandomDirection(params Vector2[] directions)
    {
        var randomIndex = Random.Range(0, directions.Length);
        return directions[randomIndex];
    }
    public void AshPillar()
    {
        StartCoroutine(AshPillarCoroutine());
    }
    private IEnumerator AshPillarCoroutine()
    {
        for (int i = 0; i < ashPillarCastCount; i++)
        {
            var moveDir = Random.Range(0, 2) == 0 ? 1 : -1;
            var spawnPos = transform.position + Vector3.right * (-1) * moveDir * ashPillarSpawnDistance;
            var ashPillarInstance = Instantiate(ashPillar, spawnPos, Quaternion.identity);
            ashPillarInstance.SetDirection(moveDir);

            // cast interval
            yield return new WaitForSeconds(ashPillarCastInterval);
        }
    }
    public void FirePillar()
    {
        var player = SceneContext.Current.Player;

        // debug
        var left = player.transform.position + Vector3.right * firePillarSpawnRange.Start;
        var right = player.transform.position + Vector3.right * firePillarSpawnRange.End;
        Debug.DrawLine(left + Vector3.down * 10f, left + Vector3.up * 10f, Color.red, 2f);
        Debug.DrawLine(right + Vector3.down * 10f, right + Vector3.up * 10f, Color.red, 2f);

        _usedPosX = new List<float>();

        for (int i = 0; i < firePillarCount; i++)
        {
            // reallocation count limit
            var posReallocationCount = 0;

            // calculate pillar spawn position
            float newPosXInRange;

            do
            {
                newPosXInRange = player.transform.position.x + firePillarSpawnRange.Random();
                posReallocationCount++;

            } while ((_usedPosX.Any(usedPosX => Mathf.Abs(usedPosX - newPosXInRange) <= firePillarEachDistance) ||
                      (newPosXInRange >= player.BodyCollider.bounds.min.x &&
                       newPosXInRange <= player.BodyCollider.bounds.max.x)) &&
                     posReallocationCount <= 20);

            // store posX
            _usedPosX.Add(newPosXInRange);
        }

        foreach (var posX in _usedPosX)
        {
            var spawnPosition = new Vector3(posX, firePillarSpawnHeight, 0f);
            var firePillarInstance = Instantiate(firePillar, spawnPosition, Quaternion.identity);
        }
    }

    public IEnumerator StartSkill(float duration = 1f)
    {
        yield return new WaitForSeconds(duration);      // duration이 경과한 후, 스킬을 사용한다

        // FlameBeam();
        // FireBall();
        // AshPillar();
        FirePillar();
    }

    private void OnDrawGizmosSelected()
    {        
        // 넝쿨 기둥이 생성되는 땅의 위치
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - 25f, firePillarSpawnHeight, transform.position.z),
            new Vector3(transform.position.x + 25f, firePillarSpawnHeight, transform.position.z));
    }
}
