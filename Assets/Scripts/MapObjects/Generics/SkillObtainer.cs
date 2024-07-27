using System.Collections;
using UnityEngine;

public class SkillObtainer : MonoBehaviour
{
    [SerializeField] string _skillKey;
    [SerializeField] SoundClipData _skillUISound;

    private string SaveSkillKey => _skillKey + "_isSaved";

    private void Awake()
    {
        SaveAndLoader.OnSaveStarted += SaveSkill;

        if(PersistentDataManager.GetByGlobal<bool>(SaveSkillKey))
        {
            PersistentDataManager.SetByGlobal(_skillKey, true);
        }
        else
        {
            PersistentDataManager.SetByGlobal(_skillKey, false);
        }
    }

    private void SaveSkill()
    {
        if(PersistentDataManager.GetByGlobal<bool>(_skillKey))
            PersistentDataManager.SetByGlobal(SaveSkillKey, true);

        Debug.Log("saveSkillKey: " + PersistentDataManager.GetByGlobal<bool>(SaveSkillKey));
    }

    public void ObtainSkill()
    {
        var skillToGet = PersistentDataManager.SkillOrderData.GetFromDict(_skillKey);
        var info = new SkillObtainPanel.SkillInfo();
        info.Icon = skillToGet.UnlockImage;
        info.MainText = skillToGet.Name;
        info.DetailText = skillToGet.DetailText;
        GameUIManager.OpenSkillObtainPanel(info);
        PersistentDataManager.SetByGlobal(_skillKey, true);
        StartCoroutine(PlaySoundCoroutine(0.25f));
    }
    IEnumerator PlaySoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySFX(_skillUISound);

    }
}
