using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager _instance;
    [SerializeField] ItemObtainPanel _itemObtainPanel;
    [SerializeField] float _itemObtainPanelDuration;
    [SerializeField] SkillObtainPanel _skillObtainPanel;
    [SerializeField] float _skillObtainPanelDuration;

    private void Awake()
    {
        _instance = this;
    }
    public static void OpenSkillPieceObtainPanel(ItemObtainPanel.ItemObtainInfo info)
    {
        _instance._itemObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseItemObtainPanel(_instance._itemObtainPanelDuration));
    }
    public static void OpenSkillObtainPanel(SkillObtainPanel.SkillInfo info)
    {
        _instance._skillObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseSkillObtainPanel(_instance._skillObtainPanelDuration));
    }
    IEnumerator CloseItemObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _itemObtainPanel.Close();
    }
    IEnumerator CloseSkillObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _skillObtainPanel.Close();
    }
}
