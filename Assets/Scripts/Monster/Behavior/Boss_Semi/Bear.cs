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

    public BearAttackType currentAttack;
    public BearAttackType nextAttack;

    [Space]

    public int minTargetCount;
    public int maxTargetCount;

    [Space]

    public int currentCount;
    public int targetCount;

    [Space]

    public int currentHurtCount;
    public int targetHurtCount = 3;

    [Space]

    public bool isGroggy;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        Init();
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
    public override void OnHit(AttackInfo attackInfo)
    {
        base.OnHit(attackInfo);
    }
    public override void Die()
    {
        // Disable Hit Box
        TurnOffHitBox();
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (isGroggy)
            return;

        Debug.Log("Bear OnLightEnter");

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

    public void Init()
    {
        // 1. 지진 공격까지 필요한 일반 공격 횟수
        // 2. 다음에 실행할 일반 공격 설정

        SetTargetCount();
        SetToNormalAttack();
    }
    public void AttackPreProcess()
    {
        // 현재 상태 변경
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
            SetToSpecialAttack();
        else
            SetToNormalAttack();
    }
    private void SetTargetCount()
    {
        if (minTargetCount > maxTargetCount)
            Debug.LogError("minTargetCount > maxTargetCount");
        else if (minTargetCount < 0)
            Debug.LogError("minTargetCount < 0");
        else if (maxTargetCount <= 0)
            Debug.LogError("maxTargetCount <= 0");

        // 4번 ~ 7번 공격 후 지진 공격
        targetCount = Random.Range(minTargetCount, maxTargetCount);
    }
    private void SetToNormalAttack()
    {
        int nextAttackNumber = Random.Range(1, 5); // 1 ~ 4
        nextAttack = (BearAttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
    }
    private void SetToSpecialAttack()
    {
        nextAttack = BearAttackType.EarthQuake;
        Animator.SetInteger("NextAttackNumber", (int)nextAttack);
    }
    public void HurtPreProcess()
    {
        currentHurtCount++;
        Animator.SetInteger("HurtCount", currentHurtCount);
    }
    public void HurtPostProcess()
    {
        currentHurtCount = 0;
        Animator.SetInteger("HurtCount", currentHurtCount);
    }
}