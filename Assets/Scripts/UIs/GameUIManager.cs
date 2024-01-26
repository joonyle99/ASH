using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    static GameUIManager _instance;
    [SerializeField] ItemObtainPanel _itemObtainPanel;
    [SerializeField] float _itemObtainPanelDuration;
    [SerializeField] SkillObtainPanel _skillObtainPanel;
    [SerializeField] float _skillObtainPanelDuration;

    [SerializeField] CanvasGroup _statusUIs;
    [SerializeField] Letterbox _letterbox;

    private void Awake()
    {
        _instance = this;
    }

    public static void OpenLetterbox()
    {
        _instance._letterbox.Open();
        _instance._statusUIs.alpha = 0;
    }
    public static void CloseLetterbox()
    {
        _instance._letterbox.Close();
        _instance._statusUIs.alpha = 1;
    }
    public static void OpenSkillPieceObtainPanel(ItemObtainPanel.ItemObtainInfo info)
    {
        _instance._itemObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseItemObtainPanel(_instance._itemObtainPanelDuration));
        _instance._statusUIs.alpha = 0;
    }
    public static void OpenSkillObtainPanel(SkillObtainPanel.SkillInfo info)
    {
        _instance._skillObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseSkillObtainPanel(_instance._skillObtainPanelDuration));
        _instance._statusUIs.alpha = 0;
    }
    IEnumerator CloseItemObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _instance._statusUIs.alpha = 1;
        _itemObtainPanel.Close();
    }
    IEnumerator CloseSkillObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _instance._statusUIs.alpha = 1;
        _skillObtainPanel.Close();
    }
}
