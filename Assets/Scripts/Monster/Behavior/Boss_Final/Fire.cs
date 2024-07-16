using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public sealed class Fire : BossBehaviour
{
    public enum AttackType
    {
        None = 0,

        FlameBeam,
        Fireball,
        AshPillar,
        FirePillar,
    }

    #region Variable

    [Header("――――――― Fire Behaviour ―――――――")]
    [Space]

    [Tooltip("1: FlameBeam\n2 : Fireball\n3 : AshPillar\n4 : FirePillar")]
    [SerializeField] private Range _attackTypeRange;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ FlameBeam ____")]
    [Space]

    [SerializeField] private float _flameBeamAnimDuration;

    [Header("____ Fireball ____")]
    [Space]

    [SerializeField] private float _fireballAnimDuration;

    [Header("____ AshPillar ____")]
    [Space]

    [SerializeField] private float _ashPillarAnimDuration;

    [Header("____ FirePillar ____")]
    [Space]

    [SerializeField] private float _firePillarAnimDuration;

    #endregion

    #region Function

    protected override void Awake()
    {
        base.Awake();

        foreach (var clip in Animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "ani_fireBoss_fireBeam")
                _flameBeamAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_fireBall")
                _fireballAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_ashPillar")
                _ashPillarAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_firePillar")
                _firePillarAnimDuration = clip.length;
        }

        // 공격 판독기의 대기 이벤트 등록
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
        AttackEvaluator.WaitEvent += OnAttackWaitEvent;

        // TODO: 임시
        rageTargetHurtCount = finalTargetHurtCount / 2;
    }
    protected override void Start()
    {
        base.Start();

        SetToRandomAttack();

        /*
        if (AttackEvaluator)
            AttackEvaluator.IsUsable = false;
        */

        IsGodMode = true;
    }
    private void OnDestroy()
    {
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
    }

    public override void AttackPreProcess()
    {

    }
    public override void AttackPostProcess()
    {

    }
    public override void GroggyPreProcess()
    {

    }
    public override void GroggyPostProcess()
    {

    }

    private void SetToRandomAttack()
    {
        int nextAttackNumber = (int)_attackTypeRange.Random();

        if (System.Enum.IsDefined(typeof(AttackType), nextAttackNumber))
        {
            _nextAttack = (AttackType)nextAttackNumber;
            Animator.SetInteger("NextAttackNumber", nextAttackNumber);
        }
        else
        {
            Debug.LogError("<color=red>Invalid AttackType generated</color>");
            _nextAttack = AttackType.None;
        }
    }

    public void FlameBeam_AnimEvent()
    {

    }
    public void Fireball_AnimEvent()
    {

    }
    public void AshPillar_AnimEvent()
    {

    }
    public void FirePillar_AnimEvent()
    {

    }

    private IEnumerator OnAttackWaitEvent()
    {
        switch (_nextAttack)
        {
            case AttackType.FlameBeam:
                yield return StartCoroutine(WaitEventCoroutine_FlameBeam());
                break;
            case AttackType.Fireball:
                yield return StartCoroutine(WaitEventCoroutine_Fireball());
                break;
            case AttackType.AshPillar:
                yield return StartCoroutine(WaitEventCoroutine_AshPillar());
                break;
            case AttackType.FirePillar:
                yield return StartCoroutine(WaitEventCoroutine_FirePillar());
                break;
        }
    }
    private IEnumerator WaitEventCoroutine_FlameBeam()
    {
        yield return new WaitForSeconds(_flameBeamAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_Fireball()
    {
        yield return new WaitForSeconds(_fireballAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_AshPillar()
    {
        yield return new WaitForSeconds(_ashPillarAnimDuration);
    }
    private IEnumerator WaitEventCoroutine_FirePillar()
    {
        yield return new WaitForSeconds(_firePillarAnimDuration);
    }

    #endregion
}
