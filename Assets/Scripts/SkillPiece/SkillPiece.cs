using System.Collections;
using UnityEngine;

public class SkillPiece : MonoBehaviour, ITriggerListener
{
    [SerializeField] SoundList _soundList;

    //���� ���ְ� ����°� �� �������� ����Ʈ�� ������ �̿�
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
                info.MainText = "��ų ������ ȹ���Ͽ����ϴ�.";
                info.DetailText = "���� ��ų �رݱ��� ������ �ʿ��� ��ų ���� "+(3-skillPieceCount%3).ToString()+"��";
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
