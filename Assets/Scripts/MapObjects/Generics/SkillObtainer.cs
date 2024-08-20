using System.Collections;
using UnityEngine;

public class SkillObtainer : MonoBehaviour
{
    [SerializeField] string _skillKey;
    [SerializeField] SoundClipData _skillUISound;

    private string ObtainSkillKey => _skillKey;
    private string SaveSkillKey => _skillKey + "_isSaved";

    private void Awake()
    {
        if(SceneChangeManager.Instance.SceneChangeType == SceneChangeType.ChangeMap)
        {
            PersistentDataManager.SetByGlobal(ObtainSkillKey, PersistentDataManager.GetByGlobal<bool>(ObtainSkillKey));
        }
        else
        {
            PersistentDataManager.SetByGlobal(ObtainSkillKey, PersistentDataManager.GetByGlobal<bool>(SaveSkillKey));
        }

        SaveAndLoader.OnSaveStarted += SaveSkill;
    }

    private void SaveSkill()
    {
        if(PersistentDataManager.GetByGlobal<bool>(ObtainSkillKey))
        {
            PersistentDataManager.SetByGlobal(SaveSkillKey, true);
        }
    }

    public void ObtainSkill()
    {
        var skillToGet = PersistentDataManager.SkillOrderData.GetFromDict(_skillKey);
        var info = new SkillObtainPanel.SkillInfo();
        info.Icon = skillToGet.UnlockImage;
        info.MainText = skillToGet.Name;
        info.DetailText = skillToGet.DetailText;
        GameUIManager.OpenSkillObtainPanel(info);
        PersistentDataManager.SetByGlobal(ObtainSkillKey, true);
        StartCoroutine(PlaySoundCoroutine(0.25f));
    }
    IEnumerator PlaySoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySFX(_skillUISound);
    }
}
