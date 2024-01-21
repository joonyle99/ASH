using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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

public class Bear : MonsterBehavior, ILightCaptureListener
{
    [Header("Bear")]
    [Space]

    [SerializeField] private LayerMask _skillTargetLayer;
    [SerializeField] private GameObject ImpactPrefab;
    [SerializeField] private Vector2 _playerPos;

    [Space]

    [SerializeField] private BoxCollider2D _mainCollider;

    [Header("Attack")]
    [Space]

    public BearAttackType currentAttack;
    public BearAttackType nextAttack;

    [Space]

    public int minTargetCount;
    public int maxTargetCount;

    [Space]

    public int targetCount;
    public int currentCount;

    [Header("Slash")]
    [Space]

    [SerializeField] private BoxCollider2D _slashCollider;
    [SerializeField] private float _attackPowerX = 7f;
    [SerializeField] private float _attackPowerY = 10f;
    [SerializeField] private int _attackDamage = 20;

    [Header("Body Slam")]
    [Space]

    [SerializeField] private BoxCollider2D _bodySlamCollider;
    [SerializeField] private bool _isBodySlamming;

    [Header("Stomp")]
    [Space]

    [SerializeField] private BoxCollider2D _stompCollider;
    [SerializeField] private GameObject _stalactitePrefab;
    [SerializeField] private int _stalactiteCount = 5;

    [Header("Hurt")]
    [Space]

    public int targetHurtCount = 3;
    public int currentHurtCount;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        RandomTargetCount();
        SetToRandomAttack();
    }
    protected override void Update()
    {
        base.Update();
    }
    private void FixedUpdate()
    {
        if (_isBodySlamming)
        {
            var rayCastHits = Physics2D.BoxCastAll(_bodySlamCollider.transform.position, _bodySlamCollider.bounds.size, 0f, Vector2.zero, 0.0f, _skillTargetLayer);

            foreach (var rayCastHit in rayCastHits)
            {
                IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;

                var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();
                foreach (var listener in listeners)
                {
                    Vector2 forceVector = new Vector2(_attackPowerX * 2f * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), _attackPowerY * 1.5f);

                    var result = listener.OnHit(new AttackInfo(_attackDamage, forceVector, AttackType.SkillAttack));
                    if (result == IAttackListener.AttackResult.Success)
                        attackResult = IAttackListener.AttackResult.Success;
                }

                if (attackResult == IAttackListener.AttackResult.Success)
                {
                    Instantiate(ImpactPrefab, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
                }
            }
        }
    }
    protected override void SetUp()
    {
        base.SetUp();
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
        StartHitTimer();
        IncreaseHurtCount();
        GetComponent<SoundList>().PlaySFX("SE_Hurt");

        // Hurt
        if (currentHurtCount >= targetHurtCount)
        {
            CurHp -= 10000;

            // Die
            if (CurHp <= 0)
            {
                CurHp = 0;
                Animator.SetTrigger("Die");

                return IAttackListener.AttackResult.Success;
            }

            Animator.SetTrigger("Hurt");
            InitializeHurtCount();
        }

        return IAttackListener.AttackResult.Success;
    }
    public override void Die()
    {
        // Disable Hit Box
        TurnOffHitBox();
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (IsGroggy)
            return;

        // 그로기 상태로 진입
        Animator.SetTrigger("Groggy");
    }
    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        // Debug.Log("Bear OnLightStay");
    }
    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        // Debug.Log("Bear OnLightExit");
    }

    // base
    public void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        currentAttack = nextAttack;

        if (currentAttack is BearAttackType.Null || nextAttack is BearAttackType.Null)
        {
            Debug.LogError("BearAttackType is Null");
            return;
        }

        if (currentAttack is BearAttackType.EarthQuake)
            currentCount = 0;
        else
            currentCount++;

    }
    public void AttackPostProcess()
    {
        if (currentCount >= targetCount)
        {
            SetToEarthQuake();
            RandomTargetCount();
        }
        else
            SetToRandomAttack();
    }
    private void RandomTargetCount()
    {
        if (minTargetCount > maxTargetCount)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (minTargetCount < 0)
            Debug.LogError("minTargetCount < 0");
        else if (maxTargetCount <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // Debug.Log("RandomTargetCount");

        // 4번 ~ 7번 공격 후 지진 공격
        targetCount = Random.Range(minTargetCount, maxTargetCount);
    }
    private void SetToRandomAttack()
    {
        // Debug.Log("SetToRandomAttack");

        int nextAttackNumber = Random.Range(4, 5); // 1 ~ 4
        nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToEarthQuake()
    {
        // Debug.Log("SetToEarthQuake");

        nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)nextAttack);
    }
    public void IncreaseHurtCount()
    {
        currentHurtCount++;
        Animator.SetInteger("HurtCount", currentHurtCount);
    }
    public void InitializeHurtCount()
    {
        currentHurtCount = 0;
        Animator.SetInteger("HurtCount", currentHurtCount);
    }

    // skill
    public void Slash01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;

        // 스킬 시전 시 플레이어의 위치를 기억
        if (RecentDir > 0)
        {
            if (playerPos.x > transform.position.x)
                _playerPos = playerPos;
        }
        else if (RecentDir < 0)
        {
            if (playerPos.x < transform.position.x)
                _playerPos = playerPos;
        }
    }
    public void Slash02_AnimEvent()
    {
        Debug.DrawRay(_playerPos, Vector2.up, Color.red, 0.15f);
        Debug.DrawRay(_playerPos, Vector2.down, Color.red, 0.15f);
        Debug.DrawRay(_playerPos, Vector2.right, Color.red, 0.15f);
        Debug.DrawRay(_playerPos, Vector2.left, Color.red, 0.15f);

        RaycastHit2D[] rayCastHits = Physics2D.BoxCastAll(_playerPos, _slashCollider.bounds.size, 0f, Vector2.zero, 0.0f, _skillTargetLayer);
        foreach (var rayCastHit in rayCastHits)
        {
            IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;

            var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();
            foreach (var listener in listeners)
            {
                Vector2 forceVector = new Vector2(_attackPowerX * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), _attackPowerY);

                var result = listener.OnHit(new AttackInfo(_attackDamage, forceVector, AttackType.SkillAttack));
                if (result == IAttackListener.AttackResult.Success)
                    attackResult = IAttackListener.AttackResult.Success;
            }

            if (attackResult == IAttackListener.AttackResult.Success)
            {
                Instantiate(ImpactPrefab, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
            }
        }
    }
    public void BodySlam01_AnimEvent()
    {
        var playerPos = SceneContext.Current.Player.transform.position;
        var dir = System.Math.Sign(playerPos.x - transform.position.x);
        SetRecentDir(dir);
    }
    public void BodySlam02_AnimEvent()
    {
        _isBodySlamming = true;
        Rigidbody.AddForce(300f * Vector2.right * RecentDir, ForceMode2D.Impulse);
    }
    public void BodySlam03_AnimEvent()
    {
        Debug.Log("Velocity Zero");
        Rigidbody.velocity = Vector2.zero;
        _isBodySlamming = false;
    }
    public void Stomp01_AnimEvent()
    {
        // 발 구르기는 한 프레임만 RayCast
        RaycastHit2D[] rayCastHits = Physics2D.BoxCastAll(_stompCollider.transform.position, _stompCollider.bounds.size, 0f, Vector2.zero, 0.0f, _skillTargetLayer);

        foreach (var rayCastHit in rayCastHits)
        {
            IAttackListener.AttackResult attackResult = IAttackListener.AttackResult.Fail;

            var listeners = rayCastHit.rigidbody.GetComponents<IAttackListener>();
            foreach (var listener in listeners)
            {
                Vector2 forceVector = new Vector2(_attackPowerX * Mathf.Sign(rayCastHit.transform.position.x - transform.position.x), _attackPowerY);

                var result = listener.OnHit(new AttackInfo(_attackDamage, forceVector, AttackType.SkillAttack));
                if (result == IAttackListener.AttackResult.Success)
                    attackResult = IAttackListener.AttackResult.Success;
            }

            if (attackResult == IAttackListener.AttackResult.Success)
            {
                Instantiate(ImpactPrefab, rayCastHit.point + Random.insideUnitCircle * 0.3f, Quaternion.identity);
            }
        }

        for(int i = 0; i < _stalactiteCount; i++)
        {
            StartCoroutine(CreateObject());
        }
    }

    public IEnumerator CreateObject()
    {
        var fallingStartTime = Random.Range(0.2f, 1.5f);

        yield return new WaitForSeconds(fallingStartTime);

        // 종유석을 랜덤 위치에 생성한다
        Vector2 randomPos = Vector2.zero;
        if (Random.value > 0.5f)
            randomPos = new Vector2(Random.Range(-150f, _mainCollider.bounds.min.x - 3.0f), 18.3f);
        else
            randomPos = new Vector2(Random.Range(_mainCollider.bounds.max.x + 3.0f, -125f), 18.3f);

        var stalactite = Instantiate(_stalactitePrefab, randomPos, Quaternion.identity);
    }
}