using System.Collections;
using UnityEngine;

public class GameUIManager : MonoBehaviour, ISceneContextBuildListener
{
    private static GameUIManager _instance;

    [Header("Status")]
    [Space]

    [SerializeField] private CanvasGroup _statusUI;
    [SerializeField] private BossKeyPanel _bossKeyPanel;

    [Header("Skill")]
    [Space]

    [SerializeField] private SkillObtainPanel _skillObtainPanel;
    [SerializeField] private float _skillObtainPanelDuration;

    [Header("Item")]
    [Space]

    [SerializeField] private ItemObtainPanel _itemObtainPanel;
    [SerializeField] private float _itemObtainPanelDuration;

    [Header("ETC")]
    [Space]

    [SerializeField] private Letterbox _letterBox;
    [SerializeField] private LifePurchasePanel _lifePurchasePanel;

    public static bool IsLifePurchasePanelOpen => _instance._lifePurchasePanel.gameObject.activeInHierarchy;

    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PersistentDataManager.UpdateValue<int>("gold", x => x + 100);
        */
    }

    public void OnSceneContextBuilt()
    {
        if (BossDungeonManager.Instance != null)
        {
            // Debug.Log("Open BossKeyPanel");

            OpenBossKeyPanel(BossDungeonManager.Instance.CurrentKeyCount);
        }
        else
        {
            // Debug.Log("Close BossKeyPanel");

            CloseBossKeyPanel();
        }
    }

    // status
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

    // skill
    public static void OpenSkillObtainPanel(SkillObtainPanel.SkillInfo info)
    {
        _instance._skillObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseSkillObtainPanel(_instance._skillObtainPanelDuration));
        _instance._statusUI.alpha = 0;
    }
    private IEnumerator CloseSkillObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _instance._statusUI.alpha = 1;
        _skillObtainPanel.Close();
    }

    // item
    public static void OpenItemObtainPanel(ItemObtainPanel.ItemObtainInfo info)
    {
        _instance._itemObtainPanel.Open(info);
        _instance.StartCoroutine(_instance.CloseItemObtainPanel(_instance._itemObtainPanelDuration));
        _instance._statusUI.alpha = 0;
    }
    private IEnumerator CloseItemObtainPanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        _instance._statusUI.alpha = 1;
        _itemObtainPanel.Close();
    }

    // letter box
    public static void OpenLetterbox(bool instant = false)
    {
        _instance._letterBox.Open(instant);
        _instance._statusUI.alpha = 0;
    }
    public static void CloseLetterbox()
    {
        _instance._letterBox.Close();
        _instance._statusUI.alpha = 1;
    }

    // life purchase
    public static void OpenLifePurchasePanel()
    {
        _instance._lifePurchasePanel.Open();
    }
}
