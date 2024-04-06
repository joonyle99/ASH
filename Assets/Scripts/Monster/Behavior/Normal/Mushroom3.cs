using UnityEngine;

public class Mushroom3 : MonsterBehavior, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    [Space]

    public static bool isCutSceneMushroom;
    public CutscenePlayer cutSceneMushroom;

    protected override void Awake()
    {
        base.Awake();

        customBasicBoxCastAttackEvent -= CutScene_Mushroom;
        customBasicBoxCastAttackEvent += CutScene_Mushroom;
    }
    public void FixedUpdate()
    {
        /*
        if (_isDevouring)
            if (CurrentState.IsAutoStateTransition)
            {
                MonsterAttackInfo devourInfo = new MonsterAttackInfo(_devourDamage, new Vector2(_devourForceX, _devourForceY));
                BasicBoxCastAttack(_devourCollider.transform.position, _devourCollider.bounds.size, devourInfo, _attackTargetLayer);
                CurrentState.ElaspedStayTime = 0f;
            }
        */
    }

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (CurrentStateIs<Monster_IdleState>())
        {
            // 빛에 닿으면 숨는다
            Animator.SetTrigger("Hide");
        }
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime += Time.deltaTime;

        if (_elapsedDieTime > _targetDieTime)
        {
            Die();
        }

        // 자동 전환을 막는다
        if (CurrentState.IsAutoStateTransition)
        {
            CurrentState.ElaspedStayTime = 0f;
        }
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
    }

    public void CutScene_Mushroom()
    {
        if (!isCutSceneMushroom && cutSceneMushroom)
        {
            isCutSceneMushroom = true;
            cutSceneMushroom.Play();
        }
    }
}
