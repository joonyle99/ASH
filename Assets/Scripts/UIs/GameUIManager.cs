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
        info.MainText = "��ų ������ ȹ���Ͽ����ϴ�.";
        info.DetailText = "���� ��ų �رݱ��� ������ �ʿ��� ��ų ���� 99��";
        _instance._itemObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseItemObtainPanel(_instance._itemObtainPanelDuration));
    }
    IEnumerator CloseItemObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _itemObtainPanel.Close();
    }
}
