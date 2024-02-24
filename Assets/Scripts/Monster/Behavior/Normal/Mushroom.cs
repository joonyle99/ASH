using UnityEngine;

public class Mushroom : MonsterBehavior
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private BoxCollider2D _devourCollider;
    [SerializeField] private int _devourDamage = 5;
    [SerializeField] private float _devourForceX = 30f;
    [SerializeField] private float _devourForceY = 10f;
    [SerializeField] private bool _isDevouring;

    protected override void Awake()
    {
        base.Awake();

        customBoxCastAttackEvent -= KillPlayer;
        customBoxCastAttackEvent += KillPlayer;
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
            if (!CautionEvaluator.IsTargetWithinRangePlus())
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

    protected override void FixedUpdate()
    {
        if(_isDevouring)
        {
            MonsterAttackInfo devourInfo = new MonsterAttackInfo(_devourDamage, new Vector2(_devourForceX, _devourForceY));
            BoxCastAttack(_devourCollider.transform.position, _devourCollider.bounds.size, devourInfo, _attackTargetLayer);
        }
    }

    public void DevourStart_AnimEvent()
    {
        _isDevouring = true;
    }
    public void DevourEnd_AnimEvent()
    {
        _isDevouring = false;
    }

    public void KillPlayer()
    {
        SceneContext.Current.Player.TriggerInstantRespawn(1);
    }
}
