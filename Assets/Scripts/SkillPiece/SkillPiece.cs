using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPiece : MonoBehaviour, ITriggerListener
{
    //���� ���ְ� ����°� �� �������� ����Ʈ�� ������ �̿�
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            PersistentDataManager.UpdateValue<int>("skillPiece", x => x + 1);
            Destruction.Destruct(gameObject);
            GameUIManager.OpenSkillPieceObtainPanel();
        }
    }
}
