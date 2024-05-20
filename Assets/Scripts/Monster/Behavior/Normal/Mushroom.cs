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
            Debug.Log("Hide State�� �����Ѵ�");

            // ���� ������ ���´�
            SetAnimatorTrigger("Hide");
        }

        _elapsedDieTime += Time.deltaTime;

        // ���� �ð��� ������ �״´�
        if (_elapsedDieTime > _targetDieTime)
            CurHp -= monsterData.MaxHp;

        // �ڵ� ��ȯ�� ���´�
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
