using UnityEngine;

public enum BearAttackType
{
    Null = 0,
    Slash,
    BodySlam,
    Stomp,
    EarthQuake
}

public class Bear : MonsterBehavior
{
    [Header("Bear")]
    [Space]

    public int normalAttackCount;
    public int earthQuakableCount;

    public BearAttackType currentAttackType;
    public int currentAttackNumber;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        InitAttack();
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
        //
        return IAttackListener.AttackResult.Success;
    }

    public override void Die()
    {
        // Disable Hit Box
        TurnOffHitBox();
    }

    public void AttackProcess()
    {
        // 예외 처리
        if (currentAttackType == BearAttackType.Null)
            Debug.LogError("BearAttackType is Null");

        // 공격 타입에 따른 처리
        if (currentAttackType != BearAttackType.EarthQuake)
            NormalAttackProcess();
        else
            EarthQuakeAttackProcess();
    }

    private void NormalAttackProcess()
    {
        Debug.Log("Normal Attack Process");

        normalAttackCount++;

        if (normalAttackCount >= earthQuakableCount)
        {
            normalAttackCount = 0;

            // 공격 타입을 '지진 공격'으로 변경
            SetAttackType(BearAttackType.EarthQuake);

            return;
        }

        // 공격 타입을 '일반 공격 중 랜덤'으로 변경
        SetRandomAttackInNormal();
    }

    private void EarthQuakeAttackProcess()
    {
        Debug.Log("EarthQuake Attack Process");

        // 지진 공격 이후, 공격 초기화
        InitAttack();
    }

    private void InitAttack()
    {
        // 다음을 초기화
        // 1. 지진 공격까지 필요한 일반 공격 횟수
        // 2. 현재 실행할 일반 공격 설정

        SetRandomEarthQuakableCount(4, 8); // 4 ~ 7
        SetRandomAttackInNormal(); // 1 ~ 3
    }

    private void SetRandomEarthQuakableCount(int min, int max)
    {
        earthQuakableCount = Random.Range(min, max);
    }

    private void SetRandomAttackInNormal()
    {
        int currentAttackNum = Random.Range(1, 4);

        currentAttackType = (BearAttackType)currentAttackNum;
        Animator.SetInteger("CurrentAttackNum", currentAttackNum);
    }

    private void SetAttackType(BearAttackType attackType)
    {
        currentAttackType = attackType;
        Animator.SetInteger("CurrentAttackNum", (int)attackType);
    }
}