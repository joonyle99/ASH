using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using joonyle99;
using UnityEngine;

public sealed class Fire : BossBehaviour
{
    public enum AttackType
    {
        None = 0,

        FlameBeam,
        Fireball,
        AshPillar,
        FirePillar,
    }

    public enum FireBallDirType
    {
        Down,           // 아래 (북 -> 남)
        DiagonalLeft,   // 왼쪽 아래 대각선 (북동 -> 남서)
        DiagonalRight,  // 오른쪽 아래 대각선 (북서 -> 남동)
    }
    public struct FireBallInfo
    {
        public FireBallDirType FireballDirType;

        public Vector3 SpawnPoint;
        public Vector3 Direction;
        public float Rotation;

        public FireBallInfo(FireBallDirType fireballDirType)
        {
            FireballDirType = fireballDirType;

            var cameraController = SceneContext.Current.CameraController;
            if (cameraController == null)
            {
                Debug.LogError($"cameraController is invalid");

                SpawnPoint = default;
                Direction = default;
                Rotation = default;

                return;
            }

            switch (FireballDirType)
            {
                case FireBallDirType.Down:
                    SpawnPoint = SceneContext.Current.CameraController.TopMiddle;
                    Direction = new Vector3(0f, -1f, 0f).normalized;
                    Rotation = 90f;
                    break;
                case FireBallDirType.DiagonalLeft:
                    SpawnPoint = (SceneContext.Current.CameraController.RightTop + SceneContext.Current.CameraController.TopMiddle) / 2f;
                    Direction = new Vector3(-1f, -1f, 0f).normalized;
                    Rotation = -45f;
                    break;
                case FireBallDirType.DiagonalRight:
                    SpawnPoint = (SceneContext.Current.CameraController.LeftTop + SceneContext.Current.CameraController.TopMiddle) / 2f;
                    Direction = new Vector3(1f, -1f, 0f).normalized;
                    Rotation = 45f;
                    break;
                default:
                    SpawnPoint = default;
                    Direction = default;
                    Rotation = default;
                    break;
            }
        }
    }

    public enum AshPillarDirType
    {
        LeftToRight,        // 왼쪽에서 오른쪽
        RightToLeft,        // 오른쪽에서 왼쪽
    }
    public struct AshPillarInfo
    {
        public AshPillarDirType AshPillarDirType;

        public Vector3 SpawnPoint;
        public Vector3 Direction;
        public Vector3 DestroyPoint
        {
            get
            {
                if (AshPillarDirType == AshPillarDirType.LeftToRight)
                {
                    return SceneContext.Current.CameraController.RightMiddle;
                }
                else if (AshPillarDirType == AshPillarDirType.RightToLeft)
                {
                    return SceneContext.Current.CameraController.LeftMiddle;
                }
                else
                {
                    Debug.LogError($"AshPillarType is invalid");
                    return Vector3.zero;
                }
            }
        }

        public AshPillarInfo(AshPillarDirType ashPillarDirType)
        {
            AshPillarDirType = ashPillarDirType;

            var cameraController = SceneContext.Current.CameraController;
            if (cameraController == null)
            {
                Debug.LogError($"cameraController is invalid");

                SpawnPoint = default;
                Direction = default;

                return;
            }

            switch (AshPillarDirType)
            {
                case AshPillarDirType.LeftToRight:
                    SpawnPoint = SceneContext.Current.CameraController.LeftMiddle;
                    Direction = Vector3.right;
                    break;
                case AshPillarDirType.RightToLeft:
                    SpawnPoint = SceneContext.Current.CameraController.RightMiddle;
                    Direction = Vector3.left;
                    break;
                default:
                    SpawnPoint = default;
                    Direction = default;
                    break;
            }
        }
    }

    #region Variable

    [Header("――――――― Fire Behaviour ―――――――")]
    [Space]

    [Tooltip("1: FlameBeam\n2 : Fireball\n3 : AshPillar\n4 : FirePillar")]
    [SerializeField] private AttackType _firstAttack;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ Teleport ____")]
    [Space]

    [SerializeField] private bool _isTeleportTurn;
    public bool IsTeleportTurn
    {
        get => _isTeleportTurn;
        set => _isTeleportTurn = value;
    }

    [Header("____ FlameBeam ____")]
    [Space]

    [SerializeField] private Fire_FlameBeam _flameBeam;
    [SerializeField] private Transform _flameBeamSpawnPoint;

    [Space]

    [SerializeField] private int _flameBeamCount = 8;                               // each flame beam count
    [SerializeField] private int _flameBeamCastCount = 3;                           // flame beam cast count

    [Space]

    [SerializeField] private float _flameBeamInterval = 1.5f;

    private float _flameBeamAngle;                                                  // each flame beam angle
    private float _flameBeamAngleEachCast;                                          // flame beam cast angle

    private float _flameBeamAnimDuration;
    private Coroutine _flameBeamCoroutine;

    [Header("____ Fireball ____")]
    [Space]

    [SerializeField] private Fire_FireBall _fireBall;

    [Space]

    [SerializeField] private int _fireBallCount = 8;
    [SerializeField] private int _fireBallCastCount = 5;

    [Space]

    [SerializeField] private float _fireBallSpeed = 20f;
    [SerializeField] private float _fireBallCastInterval = 1.5f;

    private float _fireBallAnimDuration;
    private Coroutine _fireBallCoroutine;

    [Header("____ AshPillar ____")]
    [Space]

    [SerializeField] private Fire_AshPillar _ashPillar;

    [Space]

    [SerializeField] private int _ashPillarCastCount = 3;
    [SerializeField] private float _ashPillarSpeed = 10f;

    private float _ashPillarAnimDuration;
    private Coroutine _ashPillarCoroutine;

    [Header("____ FirePillar ____")]
    [Space]

    [SerializeField] private Fire_FirePillar _firePillar;
    [SerializeField] private ShakePreset _firePillarShake;

    [Space]

    [SerializeField] private int _firePillarCount = 8;

    [Space]

    [SerializeField] private float _firePillarSpawnHeight;
    [SerializeField] private float _firePillarFarDist;
    [SerializeField] private float _firePillarEachDist = 1f;

    private float _firePillarAnimDuration;
    private Coroutine _firePillarCoroutine;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_fireBoss_fireBeam")
                _flameBeamAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_fireBall")
                _fireBallAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_ashPillar")
                _ashPillarAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_firePillar")
                _firePillarAnimDuration = clip.length;
        }

        // 공격 판독기의 대기 이벤트 등록
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;

        // 텔레포트 전이 이벤트 등록
        AnimTransitionEvent -= HandleTeleportTransition;
        AnimTransitionEvent += HandleTeleportTransition;

        InitSkillVariable();

        // TEMP
        rageTargetHurtCount = finalTargetHurtCount - 5;

        // TEMP
        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            particle.gameObject.SetActive(false);
        }
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();

        IsGodMode = false;
    }
    private void OnDestroy()
    {
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AnimTransitionEvent -= HandleTeleportTransition;
    }

    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        currentAttackCount++;

        // 2번째 공격 다음은 텔레포트 턴이다
        if (currentAttackCount % 2 == 0)
        {
            IsTeleportTurn = true;
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();

        if (IsTeleportTurn)
        {
            IsTeleportTurn = false;
        }
    }
    public override void GroggyPreProcess()
    {

    }
    public override void GroggyPostProcess()
    {

    }

    // basic
    private void SetToFirstAttack()
    {
        int firstAttackNumber = (int)_firstAttack;
        _nextAttack = (AttackType)firstAttackNumber;
        Animator.SetInteger("NextAttackNumber", firstAttackNumber);
    }
    private void SetToNextAttack()
    {
        System.Random random = new System.Random();

        var nextAttackNumber =
            random.RangeExcept((int)AttackType.FlameBeam, (int)AttackType.FirePillar + 1, (int)_currentAttack);

        // 다음 공격 타입을 설정합니다.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    // skill
    private void InitSkillVariable()
    {
        // flame beam
        _flameBeamAngle = 360f / _flameBeamCount; // 360 / 8 = 45
        _flameBeamAngleEachCast = _flameBeamAngle / _flameBeamCastCount;  // 45 / 3 = 15
    }
    private IEnumerator FlameBeamCoroutine()
    {
        var currentNumber = 0;

        for (int castCount = 0; castCount < _flameBeamCastCount; castCount++)
        {
            var defaultAngle = _flameBeamAngleEachCast * currentNumber;

            for (int number = 0; number < _flameBeamCount; number++)
            {
                var eachAngle = (number * _flameBeamAngle + defaultAngle) % 360f; // 0 ~ 360

                var flameBeam = Instantiate(_flameBeam, _flameBeamSpawnPoint.position, Quaternion.Euler(0f, 0f, eachAngle));
                flameBeam.SetActor(this);
                flameBeam.ExecuteDissolveEffect();
            }

            currentNumber++;

            yield return new WaitForSeconds(_flameBeamInterval);
        }

        StopTargetCoroutine(ref _flameBeamCoroutine);
    }
    private IEnumerator FireBallCoroutine()
    {
        for (int i = 0; i < _fireBallCastCount; i++)
        {
            FireBallDirType dirType = Util.RangeMinMaxInclusive(FireBallDirType.Down, FireBallDirType.DiagonalRight);
            FireBallInfo info = new FireBallInfo(dirType);

            var fireBall = Instantiate(_fireBall, info.SpawnPoint, Quaternion.identity);
            fireBall.SetActor(this);

            var fireBallParticle = fireBall.GetComponent<ParticleSystem>();

            // module
            var mainModule = fireBallParticle.main;
            var velocityModule = fireBallParticle.velocityOverLifetime;
            var emissionModule = fireBallParticle.emission;

            // main module
            mainModule.startRotation = new ParticleSystem.MinMaxCurve(info.Rotation * Mathf.Deg2Rad);

            // velocity module
            velocityModule.x = new ParticleSystem.MinMaxCurve(info.Direction.x);
            velocityModule.y = new ParticleSystem.MinMaxCurve(info.Direction.y);
            velocityModule.speedModifier = new ParticleSystem.MinMaxCurve(_fireBallSpeed);

            // emission module
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[_fireBallCount];
            for (int j = 0; j < _fireBallCount; j++)
                bursts[j] = new ParticleSystem.Burst(_time: 0.1f * j, _count: new ParticleSystem.MinMaxCurve(1));
            emissionModule.SetBursts(bursts);

            // play particle
            fireBallParticle.Play();

            // cast interval
            yield return new WaitForSeconds(_fireBallCastInterval);
        }

        StopTargetCoroutine(ref _fireBallCoroutine);
    }
    private IEnumerator AshPillarCoroutine()
    {
        for (int i = 0; i < _ashPillarCastCount; i++)
        {
            AshPillarDirType dirType = (i == 0)
                ? AshPillarDirType.RightToLeft
                : Util.RangeMinMaxInclusive(AshPillarDirType.LeftToRight, AshPillarDirType.RightToLeft);

            AshPillarInfo info = new AshPillarInfo(dirType);

            var ashPillar = Instantiate(_ashPillar, info.SpawnPoint, Quaternion.identity);
            ashPillar.SetSpeed(_ashPillarSpeed);
            ashPillar.SetDirection(info.Direction);

            yield return new WaitUntil(() =>
                {
                    Debug.DrawLine(ashPillar.transform.position, info.DestroyPoint, Color.gray, 0.1f);

                    return (dirType == AshPillarDirType.LeftToRight)
                        ? ashPillar.transform.position.x >= info.DestroyPoint.x
                        : ashPillar.transform.position.x <= info.DestroyPoint.x;
                });

            ashPillar.DestroyImmediately();
        }

        StopTargetCoroutine(ref _ashPillarCoroutine);
    }
    private IEnumerator FirePillarCoroutine()
    {
        var player = SceneContext.Current.Player;
        var random = new System.Random();
        var usedPosX = new List<float>();

        const int maxAttempts = 20;

        for (int i = 0; i < _firePillarCount; i++)
        {
            var attempt = 0;
            float newPosX;
            do
            {
                newPosX = player.transform.position.x + (float)(random.NextDouble() * 2 - 1) * _firePillarFarDist;
                attempt++;

            } while ((usedPosX.Any(oldPosX => Mathf.Abs(oldPosX - newPosX) <= _firePillarEachDist)
                      || (newPosX >= player.BodyCollider.bounds.min.x && newPosX <= player.BodyCollider.bounds.max.x))
                     && attempt <= maxAttempts);

            usedPosX.Add(newPosX);
        }

        SceneContext.Current.CameraController.StartShake(_firePillarShake);

        yield return new WaitForSeconds(1f);

        foreach (var posX in usedPosX)
        {
            var spawnPosition = new Vector3(posX, _firePillarSpawnHeight, 0f);
            var firePillar = Instantiate(_firePillar, spawnPosition, Quaternion.identity);

            // firePillar.Effect
        }

        StopTargetCoroutine(ref _firePillarCoroutine);
    }

    // skill anim event
    public void FlameBeam_AnimEvent()
    {
        if (_flameBeamCoroutine != null)
        {
            Debug.LogError($"_flameBeamCoroutine is not null");
            return;
        }

        _flameBeamCoroutine = StartCoroutine(FlameBeamCoroutine());
    }
    public void Fireball_AnimEvent()
    {
        if (_fireBallCoroutine != null)
        {
            Debug.LogError($"_fireballCoroutine is not null");
            return;
        }

        _fireBallCoroutine = StartCoroutine(FireBallCoroutine());
    }
    public void AshPillar_AnimEvent()
    {
        if (_ashPillarCoroutine != null)
        {
            Debug.LogError($"_ashPillarCoroutine is not null");
            return;
        }

        _ashPillarCoroutine = StartCoroutine(AshPillarCoroutine());
    }
    public void FirePillar_AnimEvent()
    {
        if (_firePillarCoroutine != null)
        {
            Debug.LogError($"_firePillarCoroutine is not null");
            return;
        }

        _firePillarCoroutine = StartCoroutine(FirePillarCoroutine());
    }

    // etc
    private void StopTargetCoroutine(ref Coroutine targetCoroutine)
    {
        if (targetCoroutine != null)
        {
            StopCoroutine(targetCoroutine);
            targetCoroutine = null;
        }
        else
        {
            Debug.LogError($"targetCoroutine is already null");
            return;
        }
    }

    /// <summary>
    /// 모든 공격 애니메이션의 Duration 만큼 대기
    /// + 텔레포트 턴이라면, 텔레포트 끝날 때까지 대기 (+ 1초 추가 대기)
    ///
    /// Attack Trigger 직후 호출되는 WaitEvent (그 다음은 Duration 대기)
    /// </summary>
    private IEnumerator OnAttackWaitEvent()
    {
        switch (_nextAttack)
        {
            case AttackType.FlameBeam:
                yield return WaitEventCoroutine_FlameBeam();
                break;
            case AttackType.Fireball:
                yield return WaitEventCoroutine_Fireball();
                break;
            case AttackType.AshPillar:
                yield return WaitEventCoroutine_AshPillar();
                break;
            case AttackType.FirePillar:
                yield return WaitEventCoroutine_FirePillar();
                break;
        }

        //Debug.Log("Attack Wait Event Point 1 - Complete Animation");

        if (IsTeleportTurn)
        {
            // 텔레포트 턴이 아닐 때까지 기다린다
            yield return new WaitUntil(() => !IsTeleportTurn);

            //Debug.Log("Attack Wait Event Point 2 - Complete Teleport");

            // 텔레포트 후 추가 대기 시간
            yield return new WaitForSeconds(1f);

            //Debug.Log("Attack Wait Event Point 3 - Additional Wait Time");
        }
    }
    private IEnumerator WaitEventCoroutine_FlameBeam()
    {
        yield return new WaitForSeconds(_flameBeamAnimDuration);
        yield return new WaitUntil(() => _flameBeamCoroutine == null);
    }
    private IEnumerator WaitEventCoroutine_Fireball()
    {
        yield return new WaitForSeconds(_fireBallAnimDuration);
        yield return new WaitUntil(() => _fireBallCoroutine == null);
    }
    private IEnumerator WaitEventCoroutine_AshPillar()
    {
        yield return new WaitForSeconds(_ashPillarAnimDuration);
        yield return new WaitUntil(() => _ashPillarCoroutine == null);
    }
    private IEnumerator WaitEventCoroutine_FirePillar()
    {
        yield return new WaitForSeconds(_firePillarAnimDuration);
        yield return new WaitUntil(() => _firePillarCoroutine == null);
    }

    private bool HandleTeleportTransition(string targetTransitionParam, Monster_StateBase currentState)
    {
        // 전이할 애니메이션이 Teleport가 아니라면 즉시 전환
        if (targetTransitionParam is not "Teleport") return true;

        // 공격 중이 아닐 때까지 애니메이션 전환을 미룸
        return !IsAttacking && _fireBallCoroutine == null;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        var current = SceneContext.Current;

        if (current == null)
            return;

        var player = current.Player;

        if (player == null)
            return;

        // 불기둥이 생성되는 땅의 높이
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - 50f, _firePillarSpawnHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + 50f, _firePillarSpawnHeight, player.transform.position.z));

        // 불기둥이 생성되는 범위
        Gizmos.color = Color.yellow;
        var left = player.transform.position + Vector3.left * _firePillarFarDist;
        var right = player.transform.position + Vector3.right * _firePillarFarDist;
        Gizmos.DrawLine(left + Vector3.down * 10f, left + Vector3.up * 10f);
        Gizmos.DrawLine(right + Vector3.down * 10f, right + Vector3.up * 10f);

        // 불기둥 사이의 최소 거리
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(player.transform.position.x - _firePillarEachDist / 2f, _firePillarSpawnHeight, player.transform.position.z),
            new Vector3(player.transform.position.x - _firePillarEachDist / 2f, _firePillarSpawnHeight + 1f, player.transform.position.z));
        Gizmos.DrawLine(new Vector3(player.transform.position.x + _firePillarEachDist / 2f, _firePillarSpawnHeight, player.transform.position.z),
            new Vector3(player.transform.position.x + _firePillarEachDist / 2f, _firePillarSpawnHeight + 1f, player.transform.position.z));
    }

}
