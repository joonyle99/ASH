using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBehaviour : MonsterBehaviour
{
    #region Variable

    [field: Header("�������������� Boss Behaviour ��������������")]
    [field: Space]

    [field: SerializeField]
    public bool IsGroggy        // ���� ������ �׷α� ���� ����
    {
        get;
        set;
    }
    [field: SerializeField]
    public bool IsRage          // ���� ������ �г� ���� ����
    {
        get;
        set;
    }

    [Space]

    [SerializeField] protected GameObject luminescence;
    public bool isActiveLuminescence => luminescence.activeInHierarchy;

    [Space]

    [Tooltip("final target hurt count x boss health unit = MaxHp")]
    [SerializeField] protected int finalTargetHurtCount;        // ���� ������ �ִ� �ǰ� Ƚ��

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

    [Header("Cutscene")]
    [Space]

    [SerializeField] protected CutscenePlayerList cutscenePlayerList;

    public int TotalHitCount
    {
        get => _totalHitCount;
        private set
        {
            _totalHitCount = value;

            if (_totalHitCount == finalTargetHurtCount / 2 && !IsRage)
            {
                Debug.Log("Change RageState �ƾ� ȣ��");

                StartCoroutine(PlayCutSceneInRunning("Change RageState"));
            }
        }
    }

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        cutscenePlayerList = GetComponent<CutscenePlayerList>();
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
        if (IsGodMode || IsDead)
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

    public void SetActiveLuminescence(bool isBool)
    {
        luminescence.SetActive(isBool);
    }

    private void CheckHurtState()
    {
        if (IsDead) return;

        // �׷α� ���� �����Ǹ� �ǰ�
        if (currentHitCount >= targetHitCount)
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
    }

    public IEnumerator PlayCutSceneInRunning(string cutsceneName)
    {
        yield return new WaitUntil(CurrentStateIs<Monster_IdleState>);

        cutscenePlayerList.PlayCutscene(cutsceneName);
    }
    public IEnumerator PlayCutSceneInRunning(CutscenePlayer cutscenePlayer)
    {
        yield return new WaitUntil(CurrentStateIs<Monster_IdleState>);

        cutscenePlayerList.PlayCutscene(cutscenePlayer);
    }

    #endregion
}
