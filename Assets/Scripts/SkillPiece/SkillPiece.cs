using System.Collections;
using UnityEngine;

public class SkillPiece : MonoBehaviour, ITriggerListener
{
    [SerializeField] SoundList _soundList;

    //조각 없애고 생기는건 씬 단위에서 리스트로 보관해 이용
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            PersistentDataManager.UpdateValueByGlobal<int>("SkillPiece", x => x + 1);
            Destruction.Destruct(gameObject);
            int skillPieceCount = PersistentDataManager.GetByGlobal<int>("SkillPiece");
            if (skillPieceCount % 3 == 0)
            {
                var info = new SkillObtainPanel.SkillInfo();
                info.Icon = PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].UnlockImage;
                info.MainText = PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].Name;
                info.DetailText = "";
                CustomKeyCode keyCode = InputManager.Instance.DefaultInputSetter.
                    GetKeyCode(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].Key);
                if (keyCode != null)
                {
                    info.DetailText += keyCode.KeyCode.ToString();
                }
                info.DetailText += PersistentDataManager.SkillOrderData[skillPieceCount / 3-1].DetailText;
                PersistentDataManager.SetByGlobal<bool>(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].Key, true);
                GameUIManager.OpenSkillObtainPanel(info);
                SceneContext.Current.StartCoroutine(PlaySoundCoroutine("Skill", 0.25f));
            }
            else
            {
                var info = new ItemObtainPanel.ItemObtainInfo();
                info.MainText = "스킬 조각을 획득하였습니다.";
                info.DetailText = "다음 스킬 해금까지 앞으로 필요한 스킬 조각 "+(3-skillPieceCount%3).ToString()+"개";
                GameUIManager.OpenItemObtainPanel(info);
                SceneContext.Current.StartCoroutine(PlaySoundCoroutine("Piece", 0.25f));
            }
        }
    }
    IEnumerator PlaySoundCoroutine(string key, float delay)
    {
        yield return new WaitForSeconds(delay);
        _soundList.PlaySFX(key);
    }
}
