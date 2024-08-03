using System.Collections;
using UnityEngine;

public static class RandomExtension
{
    public static int RangeExcept(this System.Random random, int minInclusive, int maxExclusive, int except, int limitCount = 10)
    {
        if (minInclusive < 0 || maxExclusive < 0 || minInclusive >= maxExclusive)
        {
            Debug.LogError($"Invalid minInclusive or maxExclusive");
            return except;
        }

        if (except < minInclusive || except >= maxExclusive)
        {
            Debug.LogError($"Invalid except");
            return except;
        }

        var currentCount = 0;
        var result = except;

        while (result == except)
        {
            if (currentCount >= limitCount)
            {
                /*
                result = (random.Next(0, 2) == 0)
                    ? random.Next(minInclusive, except)
                    : random.Next(except, maxExclusive);
                */

                // 겹쳐도 어쩔 수 없다
                result = random.Next(minInclusive, maxExclusive);

                break;
            }

            currentCount++;

            result = random.Next(minInclusive, maxExclusive);
        }

        return result;
    }
}

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
    [SerializeField] private AttackType _firstAttack;
    [SerializeField] private AttackType _currentAttack;
    [SerializeField] private AttackType _nextAttack;

    [Header("____ Teleport ____")]
    [Space]

    [SerializeField] private bool _isTeleportTurn;
    public bool IsTeleportTurn
    {
        get => _isTeleportTurn;
        set => _isTeleportTurn = value;
    }

    [SerializeField] private float _teleportAnimDuration;

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
            if (clip.name == "ani_fireBoss_teleport")
                _teleportAnimDuration = clip.length;
            else if (clip.name == "ani_fireBoss_fireBeam")
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

        // TEMP
        rageTargetHurtCount = finalTargetHurtCount / 2;

        // TEMP
        var particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            particle.gameObject.SetActive(false);
        }
    }
    protected override void Start()
    {
        base.Start();

        SetToFirstAttack();
    }
    private void OnDestroy()
    {
        AttackEvaluator.WaitEvent -= OnAttackWaitEvent;
    }

    public override void AttackPreProcess()
    {
        // 현재 공격 상태 변경
        _currentAttack = _nextAttack;

        currentAttackCount++;

        // 2번째 공격 다음은 텔레포트 턴이다
        if (currentAttackCount % 2 == 0)
        {
            _isTeleportTurn = true;
        }
    }
    public override void AttackPostProcess()
    {
        SetToNextAttack();
    }
    public override void GroggyPreProcess()
    {

    }
    public override void GroggyPostProcess()
    {

    }

    // basic
    private void SetToFirstAttack()
    {
        int firstAttackNumber = (int)_firstAttack;
        _nextAttack = (AttackType)firstAttackNumber;
        Animator.SetInteger("NextAttackNumber", firstAttackNumber);
    }
    private void SetToNextAttack()
    {
        System.Random random = new System.Random();

        var nextAttackNumber =
            random.RangeExcept((int)AttackType.FlameBeam, (int)AttackType.FirePillar + 1, (int)_currentAttack);

        // 다음 공격 타입을 설정합니다.
        _nextAttack = (AttackType)nextAttackNumber;
        Animator.SetInteger("NextAttackNumber", nextAttackNumber);
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

    /// <summary>
    /// 모든 공격 애니메이션의 Duration 만큼 대기
    /// + 텔레포트 턴이라면, 텔레포트 끝날 때까지 대기
    ///
    /// Attack Trigger 직후 호출되는 WaitEvent (그 다음은 Duration 대기)
    /// </summary>
    private IEnumerator OnAttackWaitEvent()
    {
        switch (_nextAttack)
        {
            case AttackType.FlameBeam:
                yield return WaitEventCoroutine_FlameBeam();
                break;
            case AttackType.Fireball:
                yield return WaitEventCoroutine_Fireball();
                break;
            case AttackType.AshPillar:
                yield return WaitEventCoroutine_AshPillar();
                break;
            case AttackType.FirePillar:
                yield return WaitEventCoroutine_FirePillar();
                break;
        }

        Debug.Log("Attack Wait Event Point 1 - Complete Animation");

        if (_isTeleportTurn)
        {
            // 텔레포트 턴이 아닐 때까지 기다린다
            yield return new WaitUntil(() => !_isTeleportTurn);

            Debug.Log("Attack Wait Event Point 2 - Complete Teleport");
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
