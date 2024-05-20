using UnityEngine;

public sealed class Mushroom : MonsterBehavior, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        if (CurrentStateIs<Monster_IdleState>())
        {
            Debug.Log("Hide State로 전이한다");

            // 빛에 닿으면 숨는다
            SetAnimatorTrigger("Hide");
        }

        _elapsedDieTime += Time.deltaTime;

        // 일정 시간이 지나면 죽는다
        if (_elapsedDieTime > _targetDieTime)
            CurHp -= monsterData.MaxHp;

        // 자동 전환을 막는다
        if (CurrentStateIs<Monster_HideState>())
        {
            if (CurrentState.IsAutoStateTransition)
                CurrentState.ElapsedStayTime = 0f;
        }
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
    }
}
