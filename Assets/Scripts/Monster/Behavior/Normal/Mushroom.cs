using UnityEngine;

public class Mushroom : MonsterBehavior
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private GameObject _devourGameObject;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (CautionEvaluator)
        {
            if (!CautionEvaluator.IsTargetWithinRange())
            {
                if (CurrentStateIs<Monster_HideState>())
                {
                    Animator.SetTrigger("Idle");
                    return;
                }
            }
            // 타겟이 범위 안에 있는 경우
            else
            {
                if (CurrentStateIs<Monster_IdleState>())
                {
                    Animator.SetTrigger("Hide");
                    return;
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
        return base.OnHit(attackInfo);
    }

    public override void Die()
    {
        base.Die();
    }

    public void DevourStart_AnimEvent()
    {
        _devourGameObject.SetActive(true);
    }

    public void DevourEnd_AnimEvent()
    {
        _devourGameObject.SetActive(false);
    }
}
