using UnityEngine;

public class Mushroom : MonsterBehavior
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private GameObject _devourGameObject;


    // Etc
    // private int _countOfUpdate = 0;

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

        /*
        // TODO : temp code
        _countOfUpdate++;

        if (_countOfUpdate == 1)
            return;
        */

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

    public void IdleAnimationStart_AnimEvent()
    {
        // TODO : temp code
        //Debug.Log("Fire animation events - Idle Animation Start");
    }

    public void HideAnimationStart_AnimEvent()
    {
        // TODO : temp code
        //Debug.Log("Fire animation events - Hide Animation Start");
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
