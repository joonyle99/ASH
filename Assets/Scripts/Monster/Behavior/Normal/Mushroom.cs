using UnityEngine;

public class Mushroom : MonsterBehavior, ILightCaptureListener
{
    [Header("Mushroom")]
    [Space]

    [SerializeField] private float _targetDieTime = 4f;
    [SerializeField] private float _elapsedDieTime;

    [Space]

    public bool isDoneCutscene;
    public CutscenePlayer cutScene;

    public void OnLightEnter(LightCapturer capturer, LightSource lightSource)
    {
        if (CurrentStateIs<Monster_IdleState>())
        {
            // ���� ������ ���´�
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

        // �ڵ� ��ȯ�� ���´�
        if (CurrentState.IsAutoStateTransition)
        {
            CurrentState.ElaspedStayTime = 0f;
        }
    }

    public void OnLightExit(LightCapturer capturer, LightSource lightSource)
    {
        _elapsedDieTime = 0f;
    }

    public void Mushroom_CutScene()
    {
        if (!isDoneCutscene && cutScene)
        {
            isDoneCutscene = true;
            cutScene.Play();
        }
    }
}
