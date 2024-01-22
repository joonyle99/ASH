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
            int skillPieceCount = PersistentDataManager.Get<int>("skillPiece");
            if (skillPieceCount % 3 == 0)
            {
                var info = new SkillObtainPanel.SkillInfo();
                info.Icon = PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].UnlockImage;
                info.MainText = PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].Name;
                info.DetailText = PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].DetailText;
                GameUIManager.OpenSkillObtainPanel(info);
            }
            else
            {
                var info = new ItemObtainPanel.ItemObtainInfo();
                info.MainText = "��ų ������ ȹ���Ͽ����ϴ�.";
                info.DetailText = "���� ��ų �رݱ��� ������ �ʿ��� ��ų ���� "+(3-skillPieceCount%3).ToString()+"��";
                GameUIManager.OpenSkillPieceObtainPanel(info);

            }
        }
    }
}
