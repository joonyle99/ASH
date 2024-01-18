using System.Threading;
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

        Initialize();
    }
    protected override void Update()
    {
        base.Update();
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

        // Die
        Animator.SetTrigger("Die");

        // Hurt
        if (currentHurtCount >= targetHurtCount)
        {
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

        Debug.Log("Bear OnLightEnter");

        // �׷α� ���·� ����
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

    public void Initialize()
    {
        // Debug.Log("======== Bear Init ========");

        // 1. ���� ���ݱ��� �ʿ��� �Ϲ� ���� Ƚ��
        // 2. ������ ������ �Ϲ� ���� ����

        RandomTargetCount();
        SetToRandomAttack();

        // Debug.Log("============================");
    }
    public void AttackPreProcess()
    {
        // ���� ���� ���� ����
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
            RandomTargetCount();
            SetToEarthQuake();
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

        // 4�� ~ 7�� ���� �� ���� ����
        targetCount = Random.Range(minTargetCount, maxTargetCount);
    }
    private void SetToRandomAttack()
    {
        // Debug.Log("SetToRandomAttack");

        int nextAttackNumber = Random.Range(1, 5); // 1 ~ 4
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
}