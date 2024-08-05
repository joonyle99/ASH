using System.Collections;
using System.Collections.Generic;
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

    public enum FireBallType
    {
        Down,           // �Ʒ� (�� -> ��)
        DiagonalLeft,   // ���� �Ʒ� �밢�� (�ϵ� -> ����)
        DiagonalRight,  // ������ �Ʒ� �밢�� (�ϼ� -> ����)
    }

    public struct FireBallInfo
    {
        public FireBallType FireballType;

        public Vector3 SpawnPoint;
        public Vector3 Direction;
        public float Rotation;
        
        public FireBallInfo(FireBallType fireballType)
        {
            FireballType = fireballType;

            var cameraController = SceneContext.Current.CameraController;
            if (cameraController == null)
            {
                Debug.LogError($"cameraController is invalid");

                SpawnPoint = default;
                Direction = default;
                Rotation = default;

                return;
            }

            switch (FireballType)
            {
                case FireBallType.Down:
                    SpawnPoint = SceneContext.Current.CameraController.TopMiddle;
                    Direction = new Vector3(0f, -1f, 0f).normalized;
                    Rotation = 90f;
                    break;
                case FireBallType.DiagonalLeft:
                    SpawnPoint = SceneContext.Current.CameraController.RightTop;
                    Direction = new Vector3(-1f, -1f, 0f).normalized;
                    Rotation = -45f;
                    break;
                case FireBallType.DiagonalRight:
                    SpawnPoint = SceneContext.Current.CameraController.LeftTop;
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

    #region Variable

    [Header("�������������� Fire Behaviour ��������������")]
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

    [SerializeField] private float _teleportAnimDuration;

    [Header("____ FlameBeam ____")]
    [Space]

    [SerializeField] private Fire_FlameBeam _flameBeam;
    [SerializeField] private Transform _flameBeamSpawnPoint;

    [Space]

    [SerializeField] private int _flameBeamCount = 8;                               // each flame beam count
    [SerializeField] private float _flameBeamIntervalAngle;                         // each flame beam interval angle

    [Space]

    [SerializeField] private int _totalFlameBeamCastNumber = 3;                     // total flame beam cast count (anim event call count)
    [SerializeField] private int _currentFlameBeamCastNumber;                       // current flame beam cast count (N th anim event call)
    [SerializeField] private float _flameBeamIntervalAngleEachCast;                 // each flame beam cast interval angle

    private float _flameBeamAnimDuration;

    [Header("____ Fireball ____")]
    [Space]

    [SerializeField] private Fire_FireBall _fireBall;

    [Space]

    [SerializeField] private int _fireBallCount = 8;

    [Space]

    [SerializeField] private int _fireBallCastCount = 5;            // fixed
    [SerializeField] private float _fireBallCastInterval = 1.5f;
    
    private float _fireballAnimDuration;
    private Coroutine _fireballCoroutine;

    [Header("____ AshPillar ____")]
    [Space]

    public Fire_AshPillar ashPillar;
    public float ashPillarSpawnDistance;
    public int ashPillarCastCount;
    public float ashPillarCastInterval;
    
    private float _ashPillarAnimDuration;

    [Header("____ FirePillar ____")]
    [Space]

    public Fire_FirePillar firePillar;
    public Range firePillarSpawnRange;
    public float firePillarSpawnHeight;
    public int firePillarCount;
    public float firePillarEachDistance;

    private List<float> _usedPosX;

    private float _firePillarAnimDuration;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_fireBoss_teleport")
                _teleportAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_fireBeam")
                _flameBeamAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_fireBall")
                _fireballAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_ashPillar")
                _ashPillarAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_firePillar")
                _firePillarAnimDuration = clip.length;
        }

        // ���� �ǵ����� ��� �̺�Ʈ ���
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;

        // �ڷ���Ʈ ���� �̺�Ʈ ���
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
        // ���� ���� ���� ����
        _currentAttack = _nextAttack;

        currentAttackCount++;

        // 2��° ���� ������ �ڷ���Ʈ ���̴�
        if (currentAttackCount % 2 == 0)
        {
            _isTeleportTurn = true;
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();

        if (_currentAttack == AttackType.FlameBeam)
        {
            _currentFlameBeamCastNumber = 0;
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

        // ���� ���� Ÿ���� �����մϴ�.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }

    // skill
    private void InitSkillVariable()
    {
        // flame beam
        _flameBeamIntervalAngle = 360f / _flameBeamCount; // 360 / 8 = 45
        _flameBeamIntervalAngleEachCast = _flameBeamIntervalAngle / _totalFlameBeamCastNumber;  // 45 / 3 = 15
    }
    private IEnumerator FireBallCoroutine()
    {
        for (int i = 0; i < _fireBallCastCount; i++)
        {
            // FireBallDir
            // 
            // 1. DiagonalRight
            // 2. DiagonalLeft
            // 3. Down
            
            FireBallType type = Util.RangeMinMaxInclusive(FireBallType.Down, FireBallType.DiagonalRight);
            FireBallInfo info = new FireBallInfo(type);

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

            // emission module
            var burst = emissionModule.GetBurst(0);
            burst.count = new ParticleSystem.MinMaxCurve(_fireBallCount);

            emissionModule.SetBurst(0, burst);

            // play particle
            fireBallParticle.Play();

            // cast interval
            yield return new WaitForSeconds(_fireBallCastInterval);
        }

        StopTargetCoroutine(ref _fireballCoroutine);
    }

    // skill anim event
    public void FlameBeam_AnimEvent()
    {
        var defaultAngle = _flameBeamIntervalAngleEachCast * _currentFlameBeamCastNumber;
        //Debug.Log($"defaultAngle: {defaultAngle}");

        //string output = "eachAngle\n";

        for (int flameBeamNumber = 0; flameBeamNumber < _flameBeamCount; flameBeamNumber++)
        {
            var eachFlameBeamAngle = (flameBeamNumber * _flameBeamIntervalAngle + defaultAngle) % 360f; // 0 ~ 360
            //output += $"{eachFlameBeamAngle}\n";
            var flameBeam = Instantiate(_flameBeam, _flameBeamSpawnPoint.position, Quaternion.Euler(0f, 0f, eachFlameBeamAngle));
            flameBeam.SetActor(this);
            flameBeam.ExecuteDissolveEffect();
        }

        _currentFlameBeamCastNumber++;

        //Debug.Log(output);
    }
    public void Fireball_AnimEvent()
    {
        if (_fireballCoroutine != null)
        {
            Debug.LogError($"_fireballCoroutine is not null");
            return;
        }

        _fireballCoroutine = StartCoroutine(FireBallCoroutine());
    }
    public void AshPillar_AnimEvent()
    {

    }
    public void FirePillar_AnimEvent()
    {

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
    /// ��� ���� �ִϸ��̼��� Duration ��ŭ ���
    /// + �ڷ���Ʈ ���̶��, �ڷ���Ʈ ���� ������ ��� (+ 1�� �߰� ���)
    ///
    /// Attack Trigger ���� ȣ��Ǵ� WaitEvent (�� ������ Duration ���)
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

        Debug.Log("Attack Wait Event Point 1 - Complete Animation");

        if (_isTeleportTurn)
        {
            // �ڷ���Ʈ ���� �ƴ� ������ ��ٸ���
            yield return new WaitUntil(() => !_isTeleportTurn);

            Debug.Log("Attack Wait Event Point 2 - Complete Teleport");

            // �ڷ���Ʈ �� �߰� ��� �ð�
            yield return new WaitForSeconds(1f);

            Debug.Log("Attack Wait Event Point 3 - Additional Wait Time");
        }
    }
    private IEnumerator WaitEventCoroutine_FlameBeam()
    {
        yield return new WaitForSeconds(_flameBeamAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_Fireball()
    {
        yield return new WaitForSeconds(_fireballAnimDuration);

        // _fireballCoroutine�� null�� ������ ���
        yield return new WaitUntil(() => _fireballCoroutine == null);
    }
    private IEnumerator WaitEventCoroutine_AshPillar()
    {
        yield return new WaitForSeconds(_ashPillarAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_FirePillar()
    {
        yield return new WaitForSeconds(_firePillarAnimDuration);
    }

    private bool HandleTeleportTransition(string targetTransitionParam, Monster_StateBase currentState)
    {
        // ������ �ִϸ��̼��� Teleport�� �ƴ϶�� ��� ��ȯ
        if (targetTransitionParam is not "Teleport") return true;

        // ���� ���� �ƴ� ������ �ִϸ��̼� ��ȯ�� �̷�
        return !IsAttacking && _fireballCoroutine == null;
    }

    #endregion
}
