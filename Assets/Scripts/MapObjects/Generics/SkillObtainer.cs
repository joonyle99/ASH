using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObtainer : MonoBehaviour
{
    [SerializeField] string _skillKey;

    public void ObtainSkill()
    {
        var skillToGet = PersistentDataManager.SkillOrderData.GetFromDict(_skillKey);
        var info = new SkillObtainPanel.SkillInfo();
        info.Icon = skillToGet.UnlockImage;
        info.MainText = skillToGet.Name;
        info.DetailText = skillToGet.DetailText;
        GameUIManager.OpenSkillObtainPanel(info);
        PersistentDataManager.Set(info.MainText, true);
    }
}
