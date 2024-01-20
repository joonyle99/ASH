using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager _instance;
    [SerializeField] ItemObtainPanel _itemObtainPanel;
    [SerializeField] float _itemObtainPanelDuration;

    private void Awake()
    {
        _instance = this;
    }
    public static void OpenSkillPieceObtainPanel()
    {
        var info = new ItemObtainPanel.ItemObtainInfo();
        info.MainText = "스킬 조각을 획득하였습니다.";
        info.DetailText = "다음 스킬 해금까지 앞으로 필요한 스킬 조각 99개";
        _instance._itemObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseItemObtainPanel(_instance._itemObtainPanelDuration));
    }
    IEnumerator CloseItemObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _itemObtainPanel.Close();
    }
}
