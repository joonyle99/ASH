using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;
using joonyle99;
using UnityEngine;

public sealed class Fire : BossBehaviour
{
    #region Enum & Struct
    public enum AttackType
    {
        None = 0,

        FlameBeam,
        Fireball,
        AshPillar,
        FirePillar,

        Teleport = 10,
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

        public Vector3 SpawnPoint { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Rotation;

        public FireBallInfo(FireBallDirType fireballDirType)
        {
            FireballDirType = fireballDirType;

            // Debug.Log(fireballDirType.ToString());

            var cameraController = SceneContext.Current.CameraController;

            // joonyle99.Util.DebugDrawX(cameraController.TopMiddle);
            // Debug.Log($"{cameraController.TopMiddle}");
            // joonyle99.Util.DebugDrawX(cameraController.RightTop);
            // Debug.Log($"{cameraController.RightTop}");
            // joonyle99.Util.DebugDrawX(cameraController.LeftTop);
            // Debug.Log($"{cameraController.LeftTop}");
            // joonyle99.Util.DebugDrawX(cameraController.BottomMiddle);
            // Debug.Log($"{cameraController.BottomMiddle}");

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
                    //Debug.Log("Down");
                    SpawnPoint = SceneContext.Current.CameraController.TopMiddle;
                    Direction = new Vector3(0f, -1f, 0f).normalized;
                    Rotation = 90f;
                    break;
                case FireBallDirType.DiagonalLeft:
                    //Debug.Log("DiagonalLeft");
                    SpawnPoint = (SceneContext.Current.CameraController.RightTop + SceneContext.Current.CameraController.TopMiddle) / 2f;
                    Direction = new Vector3(-1f, -1f, 0f).normalized;
                    Rotation = -45f;
                    break;
                case FireBallDirType.DiagonalRight:
                    //Debug.Log("DiagonalRight");
                    SpawnPoint = (SceneContext.Current.CameraController.LeftTop + SceneContext.Current.CameraController.TopMiddle) / 2f;
                    Direction = new Vector3(1f, -1f, 0f).normalized;
                    Rotation = 45f;
                    break;
                default:
                    //Debug.Log("default");
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
    #endregion

    #region Variable

    [Header("――――――― Fire Behaviour ―――――――")]
    [Space]

    [Header("____ Attributes ____")]
    [Space]

    [SerializeField] private int _shouldLanternAttackedCount = 5;
    [SerializeField] private int _currentLanternAttackedCount = 0;
    public int LanternAttackCount
    {
        get => _currentLanternAttackedCount;
        set
        {
            _currentLanternAttackedCount = value;

            Debug.Log("call lantern attack cutscene");

            // StartCoroutine(PlayCutSceneInRunning("LanternAttack_" + _currentLanternAttackedCount.ToString()));
            PlayCutSceneImmediately("LanternAttack_" + _currentLanternAttackedCount.ToString());
        }
    }

    [Space]

    [SerializeField] private float _extraAttackCooldown = 1f;

    [Tooltip("1: FlameBeam\n2 : Fireball\n3 : AshPillar\n4 : FirePillar")]
    [SerializeField] private AttackType _firstAttack;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ Teleport ____")]
    [Space]

    [SerializeField] private bool _isTeleporting;
    public bool IsTeleporting => _isTeleporting;

    [Space]

    [SerializeField] private ParticleHelper _teleportPreEffect;
    public ParticleHelper TeleportPreEffect => _teleportPreEffect;

    [SerializeField] private ParticleHelper _teleportPostEffect;
    public ParticleHelper TeleportPostEffect => _teleportPostEffect;

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

    [Header("____ Fireball ____")]
    [Space]

    [SerializeField] private Fire_FireBall _fireBall;

    [Space]

    [SerializeField] private int _fireBallCount = 8;
    [SerializeField] private int _fireBallCastCount = 5;

    [Space]

    [SerializeField] private float _fireBallSpeed = 20f;
    [SerializeField] private float _fireBallCastInterval = 1.5f;

    private List<Fire_FireBall> _fireBallList = new List<Fire_FireBall>();

    [Header("____ AshPillar ____")]
    [Space]

    [SerializeField] private Fire_AshPillar _ashPillar;

    [Space]

    [SerializeField] private int _ashPillarCastCount = 3;
    [SerializeField] private float _ashPillarCastInterval = 0.5f;
    [SerializeField] private float _ashPillarSpeed = 10f;

    [Header("____ FirePillar ____")]
    [Space]

    [SerializeField] private Fire_FirePillar _firePillar;
    [SerializeField] private ParticleHelper _firePillarSymptom;
    [SerializeField] private ShakePreset _firePillarShake;

    [Space]

    [SerializeField] private int _firePillarCount = 8;

    [Space]

    [SerializeField] private float _symptomWaitTimePreShake = 0.2f;
    [SerializeField] private float _symptomWaitTimePostShake = 1f;

    [Space]

    [SerializeField] private float _firePillarSpawnHeight;
    [SerializeField] private float _firePillarFarDist;
    [SerializeField] private float _firePillarEachDist = 1f;

    [Header("____ Skill Process ____")]
    [Space]

    private float _flameBeamAnimDuration;
    private Coroutine _flameBeamCoroutine;
    public Coroutine flameBeamCoroutine
    {
        get => _flameBeamCoroutine;
        set
        {
            _flameBeamCoroutine = value;
            _isCastingFlameBeam = _flameBeamCoroutine != null;
        }
    }
    private bool _isCastingFlameBeam;

    private float _fireBallAnimDuration;
    private Coroutine _fireBallCoroutine;
    public Coroutine fireBallCoroutine
    {
        get => _fireBallCoroutine;
        set
        {
            _fireBallCoroutine = value;
            _isCastingFireBall = _fireBallCoroutine != null;
        }
    }
    private bool _isCastingFireBall;

    private float _ashPillarAnimDuration;
    private Coroutine _ashPillarCoroutine;
    public Coroutine ashPillarCoroutine
    {
        get => _ashPillarCoroutine;
        set
        {
            _ashPillarCoroutine = value;
            _isCastingAshPillar = _ashPillarCoroutine != null;
        }
    }
    private bool _isCastingAshPillar;

    private float _firePillarAnimDuration;
    private Coroutine _firePillarCoroutine;
    public Coroutine firePillarCoroutine
    {
        get => _firePillarCoroutine;
        set
        {
            _firePillarCoroutine = value;
            _isCastingFirePillar = _firePillarCoroutine != null;
        }
    }
    private bool _isCastingFirePillar;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        // 스킬 애니메이션(4종)의 Duration을 가져옴
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
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;
        // 텔레포트 전이 이벤트 등록
        AnimTransitionEvent += HandleTeleportTransition;

        // 스킬 변수 초기화

        // flame beam
        _flameBeamAngle = 360f / _flameBeamCount; // 360 / 8 = 45
        _flameBeamAngleEachCast = _flameBeamAngle / _flameBeamCastCount;  // 45 / 3 = 15

        _shouldLanternAttackedCount = FindObjectsOfType<BossLantern>(true).Length;
        rageTargetHurtCount = finalTargetHurtCount - _shouldLanternAttackedCount;
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();

        // 무적 상태 초기화
        IsGodMode = false;
    }
    protected override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        // CHEAT: Left Ctrl + S 키를 누르면 실행 중인 스킬 코루틴 디버그
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            PrintSkillCoroutine();
        }
#endif
    }
    private void OnDestroy()
    {
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AnimTransitionEvent -= HandleTeleportTransition;
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        var result = base.OnHit(attackInfo);

        if (result == IAttackListener.AttackResult.Fail)
        {
            return result;
        }

        if (attackInfo.Type == global::AttackType.GimmickAttack)
        {
            LanternAttackCount++;
        }

        return result;
    }
    public override void Die(bool isHitBoxDisable, bool isDeathProcess)
    {
        // 보스는 사망 이펙트를 재생하지 않는다
        base.Die(true, false);
    }

    public override void AttackPreProcess()
    {
        if (!_isTeleporting)
        {
            currentAttackCount++;
        }

        _currentAttack = _nextAttack;
    }
    public override void AttackPostProcess()
    {
        _isTeleporting = !_isTeleporting && (currentAttackCount % 2) == 0;

        if (_isTeleporting)
        {
            SetToTeleport();
        }
        else
        {
            if (IsRage)
            {
                SetToFireBall();
            }
            else
            {
                SetToNextAttack();
            }
        }
    }
    public override void GroggyPreProcess()
    {
        //throw new System.NotImplementedException();
    }
    public override void GroggyPostProcess()
    {
        //throw new System.NotImplementedException();
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
    private void SetToTeleport()
    {
        var nextAttackNumber = (int)AttackType.Teleport;

        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToFireBall()
    {
        var nextAttackNumber = (int)AttackType.Fireball;
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    private IEnumerator FlameBeamCoroutine()
    {
        var currentNumber = 0;

        //StartCoroutine(FlameBeamSoundCoroutine());

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

        flameBeamCoroutine = null;
    }
    private IEnumerator FireBallCoroutine()
    {
        for (int i = 0; i < _fireBallCastCount; i++)
        {
            // escape condition
            // if (SceneEffectManager.Instance.IsPlayingCutscene)
            // {
            //     ClearAllFireBall();
            //     fireBallCoroutine = null;
            //     yield break;
            // }

            FireBallDirType dirType = IsRage ? Math.RangeMinMaxInclusive(FireBallDirType.Down, FireBallDirType.DiagonalRight) : FireBallDirType.Down;
            FireBallInfo info = new FireBallInfo(dirType);

            // Debug.Log("FireBall 생성");

            var fireBall = Instantiate(_fireBall, info.SpawnPoint, Quaternion.identity);
            var fireBallParticle = fireBall.GetComponent<ParticleSystem>();

            // fireBall이 파괴되면 _fireBallList에서 제거
            fireBall.SetOwner(this);
            _fireBallList.Add(fireBall);

            // module
            var mainModule = fireBallParticle.main;
            var velocityModule = fireBallParticle.velocityOverLifetime;
            var emissionModule = fireBallParticle.emission;
            var triggerModule = fireBallParticle.trigger;

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

            // trigger module
            triggerModule.AddCollider(SceneContext.Current.Player.BodyCollider);

            // play particle
            fireBallParticle.Play();

            // play sound
            SoundList.PlaySFX("SE_Fire_Fireball2");

            // cast interval
            yield return new WaitForSeconds(_fireBallCastInterval);
        }

        fireBallCoroutine = null;
    }
    private IEnumerator AshPillarCoroutine()
    {
        for (int i = 0; i < _ashPillarCastCount; i++)
        {
            AshPillarDirType dirType = (i == 0)
                ? AshPillarDirType.RightToLeft
                : Math.RangeMinMaxInclusive(AshPillarDirType.LeftToRight, AshPillarDirType.RightToLeft);

            AshPillarInfo info = new AshPillarInfo(dirType);

            var ashPillar = Instantiate(_ashPillar, info.SpawnPoint, Quaternion.identity);
            ashPillar.SetSpeed(_ashPillarSpeed);
            ashPillar.SetDirection(info.Direction);

            yield return new WaitUntil(() =>
                {
                    // Debug.DrawLine(ashPillar.transform.position, info.DestroyPoint, Color.gray, 0.1f);

                    return (dirType == AshPillarDirType.LeftToRight)
                        ? ashPillar.transform.position.x >= info.DestroyPoint.x
                        : ashPillar.transform.position.x <= info.DestroyPoint.x;
                });

            ashPillar.DestroyImmediately();

            yield return new WaitForSeconds(_ashPillarCastInterval);
        }

        ashPillarCoroutine = null;
    }
    private IEnumerator FirePillarCoroutine()
    {
        var player = SceneContext.Current.Player;
        var random = new System.Random();
        var usedPosX = new List<float>();

        const int MAX_ATTEMT = 15;

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
                     && attempt <= MAX_ATTEMT);

            usedPosX.Add(newPosX);
        }

        // 전조 증상
        {
            SoundList.PlaySFX("SE_Fire_Volcano1");

            foreach (var posX in usedPosX)
            {
                var spawnPosition = new Vector3(posX, _firePillarSpawnHeight, 0f);
                var symptom = Instantiate(_firePillarSymptom, spawnPosition, Quaternion.identity);
                symptom.PlayAll();
            }

            yield return new WaitForSeconds(_symptomWaitTimePreShake);

            SceneContext.Current.CameraController.StartShake(_firePillarShake);

            yield return new WaitForSeconds(_symptomWaitTimePostShake);
        }

        // 불기둥 생성
        {
            SoundList.PlaySFX("SE_Fire_Volcano2");

            foreach (var posX in usedPosX)
            {
                var spawnPosition = new Vector3(posX, _firePillarSpawnHeight, 0f);
                var firePillar = Instantiate(_firePillar, spawnPosition, Quaternion.identity);
                firePillar.StartCoroutine(firePillar.ExecutePillar());
            }
        }

        firePillarCoroutine = null;
    }

    // TEMP..?
    private IEnumerator FlameBeamSoundCoroutine()
    {
        // flameBeam의 애니메이터를 가져온다
        var animator = _flameBeam.GetComponent<Animator>();
        var runTimeController = animator.runtimeAnimatorController;

        var SE_Fire_Flamebeam1 = 0f;
        var SE_Fire_Flamebeam2 = 0f;

        // 해당 애니메이션의 이벤트 키의 위치를 가져온다
        foreach (var clip in runTimeController.animationClips)
        {
            foreach (var animationEvent in clip.events)
            {
                if (animationEvent.functionName == "PlaySound_SE_Fire_Flamebeam1")
                {
                    Debug.Log($"SE_Fire_Flamebeam1 = {animationEvent.time}");
                    SE_Fire_Flamebeam1 = animationEvent.time;
                }
                else if (animationEvent.functionName == "PlaySound_SE_Fire_Flamebeam2")
                {
                    Debug.Log($"SE_Fire_Flamebeam2 = {animationEvent.time}");
                    SE_Fire_Flamebeam2 = animationEvent.time;
                }
            }
        }

        // 이벤트 키의 타이밍에 맞춰 사운드를 재생한다
        if (SE_Fire_Flamebeam1 > 0.01f)
        {
            Debug.Log($"SE_Fire_Flamebeam1를 {SE_Fire_Flamebeam1}초 후에 실행합니다");
            yield return new WaitForSeconds(SE_Fire_Flamebeam1);
            SoundManager.Instance.PlayCommonSFX("SE_Fire_Flamebeam1");
        }

        // 이벤트 키의 타이밍에 맞춰 사운드를 재생한다
        if (SE_Fire_Flamebeam2 > 0.01f)
        {
            Debug.Log($"SE_Fire_Flamebeam2를 {SE_Fire_Flamebeam2}초 후에 실행합니다");
            yield return new WaitForSeconds(SE_Fire_Flamebeam2);
            SoundManager.Instance.PlayCommonSFX("SE_Fire_Flamebeam2");
        }

        yield return null;
    }

    // skill anim event
    public void FlameBeam_AnimEvent()
    {
        if (flameBeamCoroutine != null)
        {
            StopCoroutine(flameBeamCoroutine);
            flameBeamCoroutine = null;
            //StopTargetSkillCoroutine(flameBeamCoroutine, "_flameBeamCoroutine");
        }

        flameBeamCoroutine = StartCoroutine(FlameBeamCoroutine());
    }
    public void Fireball_AnimEvent()
    {
        if (fireBallCoroutine != null)
        {
            ClearAllFireBall();
            StopCoroutine(fireBallCoroutine);
            fireBallCoroutine = null;
            //StopTargetSkillCoroutine(fireBallCoroutine, "_fireBallCoroutine");
        }

        fireBallCoroutine = StartCoroutine(FireBallCoroutine());
    }
    public void AshPillar_AnimEvent()
    {
        if (ashPillarCoroutine != null)
        {
            StopCoroutine(ashPillarCoroutine);
            ashPillarCoroutine = null;
            //StopTargetSkillCoroutine(ashPillarCoroutine, "_ashPillarCoroutine");
        }

        ashPillarCoroutine = StartCoroutine(AshPillarCoroutine());
    }
    public void FirePillar_AnimEvent()
    {
        if (firePillarCoroutine != null)
        {
            StopCoroutine(firePillarCoroutine);
            firePillarCoroutine = null;
            //StopTargetSkillCoroutine(firePillarCoroutine, "_firePillarCoroutine");
        }

        firePillarCoroutine = StartCoroutine(FirePillarCoroutine());
    }

    public void Fireball_SoundAnimEvent()
    {
        SoundList.PlaySFX("SE_Fire_Fireball1");
    }

    // skill effect anim event
    public void TeleportPreEffect_AnimEvent()
    {
        var preAshEffect = Instantiate(TeleportPreEffect, CenterOfMass.position, Quaternion.identity);
    }
    public void TeleportPostEffect_AnimEvent()
    {
        var postAshEffect = Instantiate(TeleportPostEffect, CenterOfMass.position, Quaternion.identity);

        SoundList.PlaySFX("SE_Fire_Move");
    }

    // etc
    private void StopTargetSkillCoroutine(Coroutine targetCoroutine, string coroutineName)
    {
        if (targetCoroutine != null)
        {
            Debug.Log($"{coroutineName} is not null - stop this coroutine");

            StopCoroutine(targetCoroutine);
            targetCoroutine = null;
        }
        else
        {
            Debug.LogError($"{coroutineName} is already null");
            return;
        }
    }
    public void StopAllSkillCoroutine()
    {
        // Debug.Log("모든 스킬을 종료합니다");

        if (flameBeamCoroutine != null)
        {
            StopCoroutine(flameBeamCoroutine);
            flameBeamCoroutine = null;
            // StopTargetSkillCoroutine(flameBeamCoroutine, "_flameBeamCoroutine");
        }

        if (_fireBallList.Count > 0)
        {
            ClearAllFireBall();
        }

        if (fireBallCoroutine != null)
        {
            StopCoroutine(fireBallCoroutine);
            fireBallCoroutine = null;
            // StopTargetSkillCoroutine(fireBallCoroutine, "_fireBallCoroutine");
        }

        if (ashPillarCoroutine != null)
        {
            StopCoroutine(ashPillarCoroutine);
            ashPillarCoroutine = null;
            // StopTargetSkillCoroutine(ashPillarCoroutine, "_ashPillarCoroutine");
        }

        if (firePillarCoroutine != null)
        {
            StopCoroutine(firePillarCoroutine);
            firePillarCoroutine = null;
            // StopTargetSkillCoroutine(firePillarCoroutine, "_firePillarCoroutine");
        }
    }

    public void RemvoeFireBall(Fire_FireBall fireball)
    {
        if (_fireBallList.Contains(fireball))
        {
            _fireBallList.Remove(fireball);
        }
    }
    private void ClearAllFireBall()
    {
        foreach (var fireball in _fireBallList)
        {
            fireball.Ps.Stop();
            fireball.Ps.Clear();
        }

        _fireBallList.Clear();
    }

    private void PrintSkillCoroutine()
    {
        string result = string.Empty;

        if (flameBeamCoroutine != null)
        {
            result += $"<color=orange>_flameBeamCoroutine</color>: {_flameBeamCoroutine}\n";
        }

        if (fireBallCoroutine != null)
        {
            result += $"<color=orange>fireBallCoroutine</color>: {fireBallCoroutine}\n";
        }

        if (ashPillarCoroutine != null)
        {
            result += $"<color=orange>ashPillarCoroutine</color>: {ashPillarCoroutine}\n";
        }

        if (firePillarCoroutine != null)
        {
            result += $"<color=orange>firePillarCoroutine</color>: {firePillarCoroutine}\n";
        }

        Debug.Log(result);
    }

    /// <summary>
    /// 모든 공격 애니메이션의 Duration 만큼 대기
    /// + 텔레포트 턴이라면, 텔레포트 끝날 때까지 대기 (+ N초 추가 대기)
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

        // yield return new WaitForSeconds(_extraAttackCooldown);
    }
    private IEnumerator WaitEventCoroutine_FlameBeam()
    {
        yield return new WaitForSeconds(_flameBeamAnimDuration);
        //yield return new WaitUntil(() => _flameBeamCoroutine == null);
    }
    private IEnumerator WaitEventCoroutine_Fireball()
    {
        yield return new WaitForSeconds(_fireBallAnimDuration);
        //yield return new WaitUntil(() => _fireBallCoroutine == null);

        //독립적인 스킬이므로 다른 스킬과 중복되어 발동될 수 있다.
        //스킬 시전 후, 추가적으로 필드에 남아있음
    }
    private IEnumerator WaitEventCoroutine_AshPillar()
    {
        yield return new WaitForSeconds(_ashPillarAnimDuration);
        //yield return new WaitUntil(() => _ashPillarCoroutine == null);

        //독립적인 스킬이므로 다른 스킬과 중복되어 발동될 수 있다.
        //스킬 시전 후, 추가적으로 필드에 남아있음
    }
    private IEnumerator WaitEventCoroutine_FirePillar()
    {
        yield return new WaitForSeconds(_firePillarAnimDuration);
        //yield return new WaitUntil(() => _firePillarCoroutine == null);
    }

    /// <summary>
    /// 애니메이션 전이 조건: 텔레포트라면 공격 중이 아닐 때까지 대기..?
    /// </summary>
    private bool HandleTeleportTransition(string targetTransitionParam, Monster_StateBase currentState)
    {
        // 전이할 애니메이션이 Teleport가 아니라면 즉시 전환
        if (targetTransitionParam is not "Teleport") return true;

        // 공격 중이 아닐 때까지 애니메이션 전환을 미룸
        return !IsAttacking && fireBallCoroutine == null;
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
