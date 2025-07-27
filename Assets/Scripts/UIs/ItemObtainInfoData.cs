using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObtainInfoData : MonoBehaviour
{
    [SerializeField] private int _remainSkillPiece;

    public void OpenItemObtainPanel()
    {
        var info = new ItemObtainPanel.ItemObtainInfo();
        info.MainText = UITranslator.GetLocalizedString("ui_obtainSkillPiece");
        info.DetailText = string.Format(UITranslator.GetLocalizedString("ui_remainSkillPiece"), _remainSkillPiece);
        GameUIManager.OpenItemObtainPanel(info);
    }
}
