using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour, ISceneContextBuildListener
{
    static GameUIManager _instance;
    [SerializeField] ItemObtainPanel _itemObtainPanel;
    [SerializeField] float _itemObtainPanelDuration;
    [SerializeField] SkillObtainPanel _skillObtainPanel;
    [SerializeField] float _skillObtainPanelDuration;
    [SerializeField] BossKeyPanel _bossKeyPanel;

    [SerializeField] LifePurchasePanel _lifePurchasePanel;

    [SerializeField] CanvasGroup _statusUIs;
    [SerializeField] Letterbox _letterbox;

    public static bool IsLifePurchasePanelOpen => _instance._lifePurchasePanel.gameObject.activeInHierarchy;
    private void Awake()
    {
        _instance = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PersistentDataManager.UpdateValue<int>("gold", x => x + 100);
    }
    public void OnSceneContextBuilt()
    {
        if (BossDungeonManager.Instance != null)
        {
            OpenBossKeyPanel(BossDungeonManager.Instance.CurrentKeyCount);
        }
        else
        {
            CloseBossKeyPanel();
        }
    }
    public static void OpenLifePurchasePanel()
    {
        _instance._lifePurchasePanel.Open();
    }

    public static void OpenBossKeyPanel(int keyCount)
    {
        _instance._bossKeyPanel.gameObject.SetActive(true);
        _instance._bossKeyPanel.SetKeyCountInstant(keyCount);
    }
    public static void CloseBossKeyPanel()
    {
        _instance._bossKeyPanel.gameObject.SetActive(false);
    }
    public static void AddBossKey()
    {
        _instance._bossKeyPanel.AddKey();
    }
    public static void OpenLetterbox(bool instant = false)
    {
        _instance._letterbox.Open(instant);
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
