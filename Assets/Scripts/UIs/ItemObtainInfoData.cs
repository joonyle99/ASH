using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObtainInfoData : MonoBehaviour
{
    [SerializeField] ItemObtainPanel.ItemObtainInfo _itemObtainInfo = new ItemObtainPanel.ItemObtainInfo();

    public void OpenItemObtainPanel()
    {
        PersistentDataManager.UpdateValueByGlobal<int>("SkillPiece", x => x + 1);

        int skillPieceCount = PersistentDataManager.GetByGlobal<int>("SkillPiece");

        if (skillPieceCount % 3 == 0)
        {
            var info = new SkillObtainPanel.SkillInfo();

            info.Icon = PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].UnlockImage;

            info.MainText = UITranslator.GetLocalizedString(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].NameId);
            info.DetailText = "";

            CustomKeyCode keyCode = InputManager.Instance.DefaultInputSetter.GetKeyCode(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].Key);

            info.DetailText += string.Format(UITranslator.GetLocalizedString(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].DetailTextId), keyCode.KeyCode.ToString());

            PersistentDataManager.SetByGlobal<bool>(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].Key, true);
            GameUIManager.OpenSkillObtainPanel(info);
        }
        else
        {
            var info = new ItemObtainPanel.ItemObtainInfo();
            info.MainText = UITranslator.GetLocalizedString("ui_obtainSkillPiece");
            info.DetailText = string.Format(UITranslator.GetLocalizedString("ui_remainSkillPiece"), 3 - skillPieceCount % 3);
            GameUIManager.OpenItemObtainPanel(info);
        }
    }
}
