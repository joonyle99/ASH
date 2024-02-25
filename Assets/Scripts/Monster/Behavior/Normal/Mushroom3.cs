using UnityEngine;

public class Mushroom3 : MonsterBehavior, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private BoxCollider2D _devourCollider;
    [SerializeField] private int _devourDamage = 5;
    [SerializeField] private float _devourForceX = 30f;
    [SerializeField] private float _devourForceY = 10f;
    [SerializeField] private bool _isDevouring;

    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

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

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        // ºû¿¡ ´êÀ¸¸é ¼û´Â´Ù
        Animator.SetTrigger("Hide");
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime += Time.deltaTime;
        if (_elapsedDieTime > _targetDieTime)
        {
            // Die
            Die();
        }

        CurrentState.ElaspedStayTime = 0f;
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
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
}
