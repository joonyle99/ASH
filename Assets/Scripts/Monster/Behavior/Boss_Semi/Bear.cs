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
        // ���� ó��
        if (currentAttackType == BearAttackType.Null)
            Debug.LogError("BearAttackType is Null");

        // ���� Ÿ�Կ� ���� ó��
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

            // ���� Ÿ���� '���� ����'���� ����
            SetAttackType(BearAttackType.EarthQuake);

            return;
        }

        // ���� Ÿ���� '�Ϲ� ���� �� ����'���� ����
        SetRandomAttackInNormal();
    }

    private void EarthQuakeAttackProcess()
    {
        Debug.Log("EarthQuake Attack Process");

        // ���� ���� ����, ���� �ʱ�ȭ
        InitAttack();
    }

    private void InitAttack()
    {
        // ������ �ʱ�ȭ
        // 1. ���� ���ݱ��� �ʿ��� �Ϲ� ���� Ƚ��
        // 2. ���� ������ �Ϲ� ���� ����

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