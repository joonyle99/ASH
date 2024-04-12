using UnityEngine;

public class Mushroom : MonsterBehavior, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (CurrentStateIs<Monster_IdleState>())
        {
            // ���� ������ ���´�
            SetAnimatorTrigger("Hide");
        }
    }

    public void OnLightStay(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime += Time.deltaTime;

        if (_elapsedDieTime > _targetDieTime)
        {
            Die(true, true);
        }

        // �ڵ� ��ȯ�� ���´�
        if (CurrentState.IsAutoStateTransition)
        {
            CurrentState.ElapsedStayTime = 0f;
        }
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
    }
}
