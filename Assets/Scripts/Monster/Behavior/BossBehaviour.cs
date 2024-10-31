using System.Collections;
using UnityEngine;

public abstract class BossBehaviour : MonsterBehaviour
{
    #region Variable

    [field: Header("�������������� Boss Behaviour ��������������")]
    [field: Space]

    [field: Header("Condition")]
    [field: SerializeField]
    public bool IsGroggy { get; set; }
    [field: SerializeField]
    public bool IsRage { get; set; }

    [Space]

    [Tooltip("[Final Target Hurt Count] x [Boss Health Unit] = MaxHp")]
    [SerializeField] protected int finalTargetHurtCount;        // ���� ������ �ִ� �ǰ� Ƚ��
    [SerializeField] protected int rageTargetHurtCount;         // �г� �����ǰ� �Ǳ� ���� �ǰ� Ƚ��

    [Space]

    [Header("Attack Count")]
    [Tooltip("Count of attacks for Ultimate Skill")]
    [SerializeField] protected RangeInt attackCountRange;       // �ñر� ����� ���� �Ϲ� ���� Ƚ�� ����
    [SerializeField] protected int targetAttackCount;           // ��ǥ �Ϲ� ���� Ƚ��
    [SerializeField] protected int currentAttackCount;          // ���� �Ϲ� ���� Ƚ��

    [Space]

    [Header("Hit Count")]
    [Tooltip("Count of hits for Groggy state")]
    [SerializeField] protected RangeInt hitCountRange;          // �׷α� ���¿��� �ǰ� ������ Ƚ�� ����
    [SerializeField] protected int targetHitCount;              // ��ǥ �ǰ� Ƚ��
    [SerializeField] protected int currentHitCount;             // ���� �ǰ� Ƚ��
    [SerializeField] private int _totalHitCount;                // �� �ǰ� Ƚ��

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

                    // TODO: ���⼭ �������� �� ������ �ϱ�

                    // StartCoroutine(PlayCutSceneInRunning("Change RageState"));
                    PlayCutSceneImmediately("Change RageState");
                }
            }
        }
    }

    [Space]

    [Header("Cutscene")]
    [SerializeField] protected bool isEndMoveProcess = false;
    [SerializeField] private float _distanceFromBoss = 6f;      // ���� ��� �� ���������� �Ÿ�

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        InitBossMonsterCondition();         // ���� ���� �ʱ�ȭ

        RandomTargetAttackCount();          // ��ǥ �Ϲ� ���� Ƚ�� ���� ����
        RandomTargetHitCount();             // ��ǥ �ǰ� Ƚ�� ���� ����
    }

    public override IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (IsDead)
            return IAttackListener.AttackResult.Fail;

        if (IsGodMode && attackInfo.Type != AttackType.GimmickAttack)
            return IAttackListener.AttackResult.Fail;

        // Hit Process
        HitProcess(attackInfo, false, false, true);

        // �ǰ� Ƚ�� ����
        TotalHitCount++;
        currentHitCount++;

        // ü�� ����
        CurHp -= MonsterDefine.BossHealthUnit;

        CheckHurtState();

        return IAttackListener.AttackResult.Success;
    }
    public override void Die(bool isHitBoxDisable, bool isDeathProcess)
    {
        // ������ ��� ����Ʈ�� ������� �ʴ´�
        base.Die(true, false);

        StartCoroutine(SlowMotionCoroutine(5f));
    }

    public abstract void AttackPreProcess();        // ���� �� ��ó�� �Լ�
    public abstract void AttackPostProcess();       // ���� �� ��ó�� �Լ�
    public abstract void GroggyPreProcess();        // �׷α� ���� �� ��ó�� �Լ�
    public abstract void GroggyPostProcess();       // �׷α� ���� �� ��ó�� �Լ�

    private void CheckHurtState()
    {
        if (IsDead) return;

        if (!IsGroggy) return;

        // �׷α� ���� �����Ǹ� �ǰ�
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

        // ���� ���ʹ� �⺻������ ���� �����̴�.
        IsGodMode = true;
    }

    // cutscene
    public virtual void ExecutePostDeathActions()
    {
        // �÷��̾� �̵� ����
        StartCoroutine(PlayerMoveCoroutine());
    }
    private IEnumerator PlayerMoveCoroutine()
    {
        isEndMoveProcess = false;

        yield return new WaitForSeconds(1f);

        // �÷��̾� ��ġ
        var player = SceneContext.Current.Player;
        var playerPosX = player.transform.position.x;
        Debug.DrawRay(player.transform.position, Vector3.down * 5f, Color.cyan, 10f);

        // �������� �÷��̾������ ����
        var BossToPlayerDir = System.Math.Sign(playerPosX - transform.position.x);
        Debug.DrawRay(transform.position + Vector3.up, Vector3.right * BossToPlayerDir * _distanceFromBoss, Color.cyan, 10f);

        // �÷��̾ �̵��� ��ǥ ��ġ
        var playerMoveTargetPosX = transform.position.x + (BossToPlayerDir) * _distanceFromBoss;
        Debug.DrawRay(new Vector3(playerMoveTargetPosX, transform.position.y, transform.position.z), Vector3.down * 5f, Color.cyan, 10f);

        // �÷��̾� �̵� ����
        var playerMoveDir = System.Math.Sign(playerMoveTargetPosX - playerPosX);
        Debug.DrawRay(player.transform.position, Vector3.right * playerMoveDir * 5f, Color.cyan, 10f);

        // �÷��̾� �̵��� ���
        yield return StartCoroutine(MoveCoroutine(playerMoveDir, playerMoveTargetPosX));

        // ���� �÷��̾ �ڵ��� �ִٸ� ������ �����ش�
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

        var prefabMonster = transform.parent.gameObject; // �������� �����Ѵ�
        if (prefabMonster.GetComponent<DestructEventCaller>()) Destruction.Destruct(prefabMonster);
        else Destroy(prefabMonster);
    }

    #endregion
}
