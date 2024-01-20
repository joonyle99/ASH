using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPiece : MonoBehaviour, ITriggerListener
{
    //조각 없애고 생기는건 씬 단위에서 리스트로 보관해 이용
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
