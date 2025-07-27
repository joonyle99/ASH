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
        info.MainText = UITranslator.GetLocalizedString(skillToGet.NameId);
        CustomKeyCode keyCode = InputManager.Instance.DefaultInputSetter.GetKeyCode(skillToGet.Key);

        info.DetailText = string.Format(UITranslator.GetLocalizedString(skillToGet.DetailTextId), keyCode.KeyCode.ToString());

        GameUIManager.OpenSkillObtainPanel(info);
        PersistentDataManager.SetByGlobal(ObtainSkillKey, true);
        StartCoroutine(PlaySoundCoroutine(0.25f));
    }
    IEnumerator PlaySoundCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySFX_SoundClipData(_skillUISound);
    }
}
