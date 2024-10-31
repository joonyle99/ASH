using System.Collections;
using UnityEngine;

public abstract class BossBehaviour : MonsterBehaviour
{
    #region Variable

    [field: Header("――――――― Boss Behaviour ―――――――")]
    [field: Space]

    [field: Header("Condition")]
    [field: SerializeField]
    public bool IsGroggy { get; set; }
    [field: SerializeField]
    public bool IsRage { get; set; }

    [Space]

    [Tooltip("[Final Target Hurt Count] x [Boss Health Unit] = MaxHp")]
    [SerializeField] protected int finalTargetHurtCount;        // 보스 몬스터의 최대 피격 횟수
    [SerializeField] protected int rageTargetHurtCount;         // 분노 상태의가 되기 위한 피격 횟수

    [Space]

    [Header("Attack Count")]
    [Tooltip("Count of attacks for Ultimate Skill")]
    [SerializeField] protected RangeInt attackCountRange;       // 궁극기 사용을 위한 일반 공격 횟수 범위
    [SerializeField] protected int targetAttackCount;           // 목표 일반 공격 횟수
    [SerializeField] protected int currentAttackCount;          // 현재 일반 공격 횟수

    [Space]

    [Header("Hit Count")]
    [Tooltip("Count of hits for Groggy state")]
    [SerializeField] protected RangeInt hitCountRange;          // 그로기 상태에서 피격 가능한 횟수 범위
    [SerializeField] protected int targetHitCount;              // 목표 피격 횟수
    [SerializeField] protected int currentHitCount;             // 현재 피격 횟수
    [SerializeField] private int _totalHitCount;                // 총 피격 횟수

    public int TotalHitCount
    {
        get => _totalHitCount;
        private set
        {
            _totalHitCount = value;

            if (IsRage == false)
            {
                if (_totalHitCount == rageTargetHurtCount)
                {
                    Debug.Log("call rage cutscene");

                    IsRage = true;
                    IsGodMode = true;

                    // TODO: 여기서 공격으로 안 들어가도록 하기

                    // StartCoroutine(PlayCutSceneInRunning("Change RageState"));
                    PlayCutSceneImmediately("Change RageState");
                }
            }
        }
    }

    [Space]

    [Header("Cutscene")]
    [SerializeField] protected bool isEndMoveProcess = false;
    [SerializeField] private float _distanceFromBoss = 6f;      // 보스 사망 후 떨어져야할 거리

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        InitBossMonsterCondition();         // 보스 몬스터 초기화

        RandomTargetAttackCount();          // 목표 일반 공격 횟수 랜덤 설정
        RandomTargetHitCount();             // 목표 피격 횟수 랜덤 설정
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsDead)
            return IAttackListener.AttackResult.Fail;

        if (IsGodMode && attackInfo.Type != AttackType.GimmickAttack)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false, true);

        // 피격 횟수 증가
        TotalHitCount++;
        currentHitCount++;

        // 체력 감소
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable, bool isDeathProcess)
    {
        // 보스는 사망 이펙트를 재생하지 않는다
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public abstract void AttackPreProcess();        // 공격 시 전처리 함수
    public abstract void AttackPostProcess();       // 공격 시 후처리 함수
    public abstract void GroggyPreProcess();        // 그로기 상태 시 전처리 함수
    public abstract void GroggyPostProcess();       // 그로기 상태 시 후처리 함수

    private void CheckHurtState()
    {
        if (IsDead) return;

        if (!IsGroggy) return;

        // 그로기 상태 해제되며 피격
        if (currentHitCount % targetHitCount == 0)
        {
            SetAnimatorTrigger("Hurt");
        }
    }
    private IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    protected void RandomTargetAttackCount()
    {
        if (attackCountRange.Start > attackCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount ----> minTargetCount must be less and equal than maxTargetCount");
        else if (attackCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0 ----> minTargetCount must be greater and equal than 0");
        else if (attackCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0 ----> maxTargetCount must be greater than 0");

        targetAttackCount = Random.Range(attackCountRange.Start, attackCountRange.End);
    }
    protected void RandomTargetHitCount()
    {
        if (hitCountRange.Start > hitCountRange.End)
            Debug.LogError("minTargetCount > maxTargetCount ----> minTargetCount must be less and equal than maxTargetCount");
        else if (hitCountRange.Start < 0)
            Debug.LogError("minTargetCount < 0 ----> minTargetCount must be greater and equal than 0");
        else if (hitCountRange.End <= 0)
            Debug.LogError("maxTargetCount <= 0 ----> maxTargetCount must be greater than 0");

        targetHitCount = Random.Range(hitCountRange.Start, hitCountRange.End);
    }
    protected void InitBossMonsterCondition()
    {
        monsterData.MaxHp = finalTargetHurtCount * MonsterDefine.BossHealthUnit;
        CurHp = monsterData.MaxHp;

        // 보스 몬스터는 기본적으로 무적 상태이다.
        IsGodMode = true;
    }

    // cutscene
    public virtual void ExecutePostDeathActions()
    {
        // 플레이어 이동 연출
        StartCoroutine(PlayerMoveCoroutine());
    }
    private IEnumerator PlayerMoveCoroutine()
    {
        isEndMoveProcess = false;

        yield return new WaitForSeconds(1f);

        // 플레이어 위치
        var player = SceneContext.Current.Player;
        var playerPosX = player.transform.position.x;
        Debug.DrawRay(player.transform.position, Vector3.down * 5f, Color.cyan, 10f);

        // 보스에서 플레이어까지의 방향
        var BossToPlayerDir = System.Math.Sign(playerPosX - transform.position.x);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.right * BossToPlayerDir * _distanceFromBoss, Color.cyan, 10f);

        // 플레이어가 이동할 목표 위치
        var playerMoveTargetPosX = transform.position.x + (BossToPlayerDir) * _distanceFromBoss;
        Debug.DrawRay(new Vector3(playerMoveTargetPosX, transform.position.y, transform.position.z), Vector3.down * 5f, Color.cyan, 10f);

        // 플레이어 이동 방향
        var playerMoveDir = System.Math.Sign(playerMoveTargetPosX - playerPosX);
        Debug.DrawRay(player.transform.position, Vector3.right * playerMoveDir * 5f, Color.cyan, 10f);

        // 플레이어 이동을 대기
        yield return StartCoroutine(MoveCoroutine(playerMoveDir, playerMoveTargetPosX));

        // 만약 플레이어가 뒤돌고 있다면 방향을 돌려준다
        if (BossToPlayerDir == player.RecentDir)
        {
            var dirForLookToBear = (-1) * playerMoveDir;
            yield return StartCoroutine(MoveCoroutine(dirForLookToBear, playerMoveTargetPosX + dirForLookToBear * 0.1f));
        }

        InputManager.Instance.ChangeToStayStillSetter();
    }
    private IEnumerator MoveCoroutine(int moveDir, float targetPosX)
    {
        var isRight = moveDir > 0;

        if (isRight)
            InputManager.Instance.ChangeToMoveRightSetter();
        else
            InputManager.Instance.ChangeToMoveLeftSetter();

        yield return new WaitUntil(() => System.Math.Abs(targetPosX - SceneContext.Current.Player.transform.position.x) < 0.1f);

        isEndMoveProcess = true;
    }
    public void DisintegrateEffect()
    {
        StartCoroutine(DisintegrateEffectCoroutine());
    }
    private IEnumerator DisintegrateEffectCoroutine()
    {
        var effect = GetComponent<DisintegrateEffect>();
        effect.Play();
        yield return new WaitUntil(() => effect.IsEffectDone);

        var prefabMonster = transform.parent.gameObject; // 프리팹을 삭제한다
        if (prefabMonster.GetComponent<DestructEventCaller>()) Destruction.Destruct(prefabMonster);
        else Destroy(prefabMonster);
    }

    #endregion
}
