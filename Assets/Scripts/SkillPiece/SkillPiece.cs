using System;
using System.Collections;
using UnityEngine;

public class SkillPiece : MonoBehaviour, ITriggerListener
{
    [SerializeField] SoundList _soundList;

    [SerializeField] CutscenePlayer _cutscenePlayer;

    //조각 없애고 생기는건 씬 단위에서 리스트로 보관해 이용
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            PersistentDataManager.UpdateValueByGlobal<int>("SkillPiece", x => x + 1);
            Destruction.Destruct(gameObject);
            int skillPieceCount = PersistentDataManager.GetByGlobal<int>("SkillPiece");
            Debug.Log($"Skill piece count : {skillPieceCount}");
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
                SceneContext.Current.StartCoroutine(PlaySoundCoroutine("Skill", 0.25f));
            }
            else
            {
                var info = new ItemObtainPanel.ItemObtainInfo();
                info.MainText = UITranslator.GetLocalizedString("ui_obtainSkillPiece");
                info.DetailText = string.Format(UITranslator.GetLocalizedString("ui_remainSkillPiece"), 3 - skillPieceCount % 3);
                GameUIManager.OpenItemObtainPanel(info);
                SceneContext.Current.StartCoroutine(PlaySoundCoroutine("Piece", 0.25f));
            }

            _cutscenePlayer?.Play();
        }
    }
    IEnumerator PlaySoundCoroutine(string key, float delay)
    {
        yield return new WaitForSeconds(delay);
        _soundList.PlaySFX(key);
    }
}
