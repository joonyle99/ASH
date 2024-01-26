using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : SemiBossBehavior, ILightCaptureListener
{
    public enum BearAttackType
    {
        Null = 0,

        // Normal Attack
        Slash_Right,
        Slash_Left,
        BodySlam,
        Stomp,

        // Special Attack
        EarthQuake = 10
    }

    [Header("Bear")]
    [Space]

    [Header("Condition")]
    [Space]

    [SerializeField] private int _finalTargetHurtCount;

    [Space]

    [SerializeField] private BearAttackType _currentAttack;
    [SerializeField] private BearAttackType _nextAttack;

    [Space]

    [SerializeField] private int _minTargetAttackCount;
    [SerializeField] private int _maxTargetAttackCount;

    [Space]

    [SerializeField] private int _targetAttackCount;
    [SerializeField] private int _currentAttackCount;
    [SerializeField] private int _targetHurtCount;
    [SerializeField] private int _currentHurtCount;

    [Header("Skill")]
    [Space]

    [SerializeField] private Bear_Slash _slashPrefab;

    [Space]

    [SerializeField] private BoxCollider2D _bodySlamCollider;
    [SerializeField] private bool _isBodySlamming;

    [Space]

    [SerializeField] private int _bodySlamDamage = 20;
    [SerializeField] private float _bodySlamForceX = 7f;
    [SerializeField] private float _bodySlamForceY = 10f;

    [Header("Stomp skill")]

    [SerializeField] private Collider2D _stompCollider;
    [SerializeField] private Bear_Stalactite _stalactitePrefab;
    [SerializeField] private int _stalactiteCount;
    [SerializeField] private Range _normalStalactiteRange;
    [SerializeField] private Range _rageStalactiteRange;
    [SerializeField] private List<float> _stalactitePosXs;
    [SerializeField] private GameObject _stompEffectPrefab;

    [Space]

    [SerializeField] private int _stompDamage = 20;
    [SerializeField] private float _stompForceX = 7f;
    [SerializeField] private float _stompForceY = 10f;

    [Header("Earthquake skill")]

    [SerializeField] private BoxCollider2D _earthQuakeCollider;
    [SerializeField] private Transform _waveSpawnPoint;
    [SerializeField] private Bear_GroundWave _waveSkillPrefab;
    [SerializeField] ShakePreset _earthquakeCameraShake;

    [Space]

    [SerializeField] private int _earthQuakeDamage = 20;
    [SerializeField] private float _earthQuakeForceX = 7f;
    [SerializeField] private float _earthQuakeForceY = 10f;

    [SerializeField] SoundList _soundList;

    private Vector2 _playerPos;
    private BoxCollider2D _bodyCollider;   // not bodyHitBox

    private int healtUnit = 10000;

    public GameObject LightingStone;

    protected override void Awake()
    {
        base.Awake();

        _bodyCollider = GetComponent<BoxCollider2D>();
    }
    protected override void Start()
    {
        base.Start();

        // init
        RandomTargetAttackCount();
        SetToRandomAttack();

        // 종유석 카운트 초기화
        SetStalactiteToNormalCount();
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsDead)
            return;

        if (CurrentStateIs<GroundMoveState>())
        {
            // ground walking
            GroundWalking();
        }

        if (CurrentStateIs<SemiBoss_AttackState>())
        {
            if (_isBodySlamming)
            {
                MonsterAttackInfo bodySlamInfo = new MonsterAttackInfo(_bodySlamDamage, new Vector2(_bodySlamForceX, _bodySlamForceY));
                BoxCastAttack(_bodySlamCollider.transform.position, _bodySlamCollider.bounds.size, bodySlamInfo, _attackTargetLayer);
                // Debug.Log("BodySlam Attack");
            }
        }
    }
    protected override void SetUp()
    {
        base.SetUp();

        MaxHp = _finalTargetHurtCount * healtUnit;
        CurHp = MaxHp;
    }
    public override void KnockBack(Vector2 forceVector)
    {
        base.KnockBack(forceVector);
    }
    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsGodMode || IsDead)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false);

        // 흑곰 전용 피격 처리
        CurHp -= healtUnit;
        IncreaseHurtCount();

        // Die
        if (CurHp <= 0)
        {
            CurHp = 0;
            Die();

            return IAttackListener.AttackResult.Success;
        }

        // Rage
        if (!IsRage)
        {
            if (CurHp <= MaxHp * 0.5f)
            {
                Animator.SetTrigger("Shout");

                return IAttackListener.AttackResult.Success;
            }
        }

        // Hurt
        if (_currentHurtCount >= _targetHurtCount)
            Animator.SetTrigger("Hurt");

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        IsDead = true;

        Animator.SetTrigger("Die");

        // Disable Hit Box
        TurnOffHitBox();
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // 그로기 상태로 진입
        Animator.SetTrigger("Groggy");

        // 꺼줘야겠다
        TurnOffLightStone();
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {

    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {

    }

    // base
    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        if (_currentAttack is BearAttackType.Null || _nextAttack is BearAttackType.Null)
        {
            Debug.LogError("BearAttackType is Null");
            return;
        }

        if (_currentAttack is BearAttackType.EarthQuake)
            InitializeAttackCount();
        else
            IncreaseAttackCount();
    }
    public override void AttackPostProcess()
    {
        if (_currentAttackCount >= _targetAttackCount)
        {
            SetToEarthQuake();
            RandomTargetAttackCount();
        }
        else
            SetToRandomAttack();
    }
    public override void GroggyPreProcess()
    {
        // 그로기 상태 진입 (더이상 손전등의 영향을 받지 않음)
        IsGroggy = true;

        // 몬스터의 MonsterBodyHit를 끈다 (플레이어를 타격할 수 없다)
        SetIsAttackableHitBox(false);
    }
    public override void GroggyPostProcess()
    {
        // 그로기 상태 종료 (이제 손전등의 영향을 받음)
        IsGroggy = false;

        // 몬스터의 MonsterBodyHit를 켠다 (플레이어를 타격할 수 있다)
        SetIsAttackableHitBox(true);

        InitializeHurtCount();

        if (IsRage)
            SetStalactiteToRageCount();
    }

    // basic
    private void RandomTargetAttackCount()
    {
        if (_minTargetAttackCount > _maxTargetAttackCount)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (_minTargetAttackCount < 0)
            Debug.LogError("minTargetCount < 0");
        else if (_maxTargetAttackCount <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // 4번 ~ 7번 공격 후 지진 공격
        _targetAttackCount = Random.Range(_minTargetAttackCount, _maxTargetAttackCount);
    }
    private void SetToRandomAttack()
    {
        int nextAttackNumber = Random.Range(1, 5); // 1 ~ 4
        _nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        _nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)_nextAttack);
    }
    public void IncreaseAttackCount()
    {
        _currentAttackCount++;
    }
    public void InitializeAttackCount()
    {
        _currentAttackCount = 0;
    }
    public void IncreaseHurtCount()
    {
        _currentHurtCount++;
    }
    public void InitializeHurtCount()
    {
        _currentHurtCount = 0;
    }
    public void SetStalactiteToNormalCount()
    {
        _stalactiteCount = Random.Range((int)_normalStalactiteRange.Start, (int)_normalStalactiteRange.End + 1);
    }
    public void SetStalactiteToRageCount()
    {
        _stalactiteCount = Random.Range((int)_rageStalactiteRange.Start, (int)_rageStalactiteRange.End + 1);
    }

    // slash
    public void Slash01_AnimEvent()
    {
        // 플레이어의 위치
        var playerPos = SceneContext.Current.Player.transform.position;

        // 몬스터가 플레이어를 바라보는 방향
        var dirBearToPlayer = System.Math.Sign(playerPos.x - transform.position.x);
        // 몬스터가 바라보는 방향에 플레이어가 있는지
        bool isPlayerinLookDirection = (dirBearToPlayer == RecentDir);

        // 바라보는 방향에 플레이어가 있다면
        if (isPlayerinLookDirection)
        {
            // 플레이어의 위치를 기억
            _playerPos = playerPos;
        }
    }
    public void Slash02_AnimEvent()
    {
        // TODO : 여기서 할퀴기 생성
        var slashEffect = Instantiate(_slashPrefab, _playerPos, Quaternion.identity);
        //Destroy(slashEffect.gameObject, 0.25f);

        // 플레이어 위치 초기화
        _playerPos = Vector2.zero;
    }

    // bodySlam
    public void BodySlam01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;
        var dir = System.Math.Sign(playerPos.x - transform.position.x);

        SetRecentDir(dir);
        // StartSetRecentDirAfterGrounded(dir);
    }
    public void BodySlam02_AnimEvent()
    {
        _isBodySlamming = true;
        Rigidbody.AddForce(60f * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam03_AnimEvent()
    {
        Rigidbody.velocity = Vector2.zero;
        _isBodySlamming = false;
    }

    // stomp
    public void Stomp01_AnimEvent()
    {
        MonsterAttackInfo stompInfo = new MonsterAttackInfo(_stompDamage, new Vector2(_stompForceX, _stompForceY));
        CastAttack(_stompCollider, _stompCollider.transform.position, _stompCollider.bounds.size, stompInfo, _attackTargetLayer);
        Instantiate(_stompEffectPrefab, _stompCollider.transform.position, Quaternion.identity);
        _soundList.PlaySFX("Stomp");
        // 종유석 생성
        for (int i = 0; i < _stalactiteCount; ++i)
        {
            _stalactitePosXs.Clear();
            _stalactitePosXs = new List<float>(_stalactiteCount);
            StartCoroutine(CreateStalactite());
        }
    }
    public IEnumerator CreateStalactite()
    {
        var fallingStartTime = Random.Range(0.5f, 1.5f);

        // 종유석을 천장에서 랜덤 위치에 생성한다
        var ceilingHeight = 18.3f;
        var bodyColliderMinX = _bodyCollider.bounds.min.x;
        var bodyColliderMaxX = _bodyCollider.bounds.max.x;
        var fromDistance = 1f;
        var toDistance = 20f;
        var posXInLeftRange = Random.Range(bodyColliderMinX - toDistance, bodyColliderMinX - fromDistance);
        var posXInRightRange = Random.Range(bodyColliderMaxX + fromDistance, bodyColliderMaxX + toDistance);

        /*
        bool isSavable = false;
        float finalPosX = 0f;

        var limitCount = 0;

        // finalPosX를 정하는 과정
        do
        {
            limitCount++;

            if (RecentDir > 0)
            {
                finalPosX = posXInRightRange;
                // Debug.DrawRay(new Vector3(bodyColliderMaxX + fromDistance, ceilingHeight), Vector2.down, Color.blue, 2f);
                // Debug.DrawRay(new Vector3(bodyColliderMaxX + toDistance, ceilingHeight), Vector2.down, Color.yellow, 2f);
            }
            else
            {
                finalPosX = posXInLeftRange;
                // Debug.DrawRay(new Vector3(bodyColliderMinX - toDistance, ceilingHeight), Vector2.down, Color.red, 2f);
                // Debug.DrawRay(new Vector3(bodyColliderMinX - fromDistance, ceilingHeight), Vector2.down, Color.green, 2f);
            }

            foreach (var posX in _stalactitePosXs)
            {
                var minX = posX - 0.3f;
                var maxX = posX + 0.3f;

                // 저장 가능 조건
                if (finalPosX < minX || finalPosX > maxX)
                {
                    isSavable = true;
                    break;
                }
            }

        } while (!isSavable || limitCount > 50);
        */

        float finalPosX = 0f;
        if (RecentDir > 0)
        {
            finalPosX = posXInRightRange;
            Debug.DrawRay(new Vector3(bodyColliderMaxX + fromDistance, ceilingHeight), Vector2.down, Color.blue, 2f);
            Debug.DrawRay(new Vector3(bodyColliderMaxX + toDistance, ceilingHeight), Vector2.down, Color.yellow, 2f);
        }
        else
        {
            finalPosX = posXInLeftRange;
            Debug.DrawRay(new Vector3(bodyColliderMinX - toDistance, ceilingHeight), Vector2.down, Color.red, 2f);
            Debug.DrawRay(new Vector3(bodyColliderMinX - fromDistance, ceilingHeight), Vector2.down, Color.green, 2f);
        }

        // 모든 종유석의 생성위치 저장
        _stalactitePosXs.Add(finalPosX);
        Debug.Log(finalPosX);

        yield return new WaitForSeconds(fallingStartTime);

        // Vector2 randomPos = (Random.value > 0.5f) ? new Vector2(posXInLeftRange, ceilingHeight) : new Vector2(posXInRightRange, ceilingHeight);
        Vector2 randomPos = new Vector2(finalPosX, ceilingHeight);
        var stalactite = Instantiate(_stalactitePrefab, randomPos, Quaternion.identity);

        // 종유석의 크기도 랜덤으로
        var scale = Random.Range(0.4f, 1f);
        stalactite.transform.localScale *= scale;
        // Debug.Log(stalactite.transform.localScale);
    }

    // earthQuake
    public void Earthquake01_AnimEvent()
    {
        MonsterAttackInfo earthQuakeInfo = new MonsterAttackInfo(_earthQuakeDamage, new Vector2(_earthQuakeForceX, _earthQuakeForceY));
        BoxCastAttack(_earthQuakeCollider.transform.position, _earthQuakeCollider.bounds.size, earthQuakeInfo, _attackTargetLayer);

        // 지면파 생성
        GenerateGroundWave();
        _soundList.PlaySFX("Earthquake");
        SceneEffectManager.Current.Camera.StartShake(_earthquakeCameraShake);
    }
    public void GenerateGroundWave()
    {
        // 2개의 지면파를 발생시킨다 (좌 / 우)
        var wave1 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave1.SetDir(Vector2.left);
        var wave2 = Instantiate(_waveSkillPrefab, _waveSpawnPoint.position, Quaternion.identity);
        wave2.SetDir(Vector2.right);
    }

    public void TurnOnLightStone()
    {
        LightingStone.SetActive(true);
    }

    public void TurnOffLightStone()
    {
        LightingStone.SetActive(false);
    }
}