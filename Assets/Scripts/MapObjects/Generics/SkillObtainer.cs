using System.Collections;
using UnityEngine;

public class SkillObtainer : MonoBehaviour
{
    [SerializeField] string _skillKey;
    [SerializeField] SoundClipData _skillUISound;

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
