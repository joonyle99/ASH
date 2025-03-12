using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// 모든 Playable Scene에는 GameCanvas와 GameUIManager가 있다
/// 따라서 싱글톤이 아닌 이 클래스를 사용하여 UI를 관리한다
/// </summary>
public class GameUIManager : MonoBehaviour, ISceneContextBuildListener
{
    private static GameUIManager _instance;
    public static GameUIManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            else
            {
                Debug.LogWarning("There is no GameUIManager Instance");
                return null;
            }
        }
    }

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

    [SerializeField] private OptionView _optionView;
    public OptionView OptionView => _optionView;

    [SerializeField] private DeadPanel _deadPanel;

    [SerializeField] private TextMeshProUGUI _sceneNameText;
    [SerializeField] private TextMeshProUGUI _debugText;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        // _sceneNameText.gameObject.SetActive(true);
    }
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PersistentDataManager.UpdateValueByGlobal<int>("gold", x => x + 100);
        */
    }

    public void OnSceneContextBuilt()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        if (GameSceneManager.IsBossDungeon1(sceneName)
            || GameSceneManager.IsBossDungeon2(sceneName))
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
        if (Instance == null) return;

        Instance._bossKeyPanel.gameObject.SetActive(true);
        Instance._bossKeyPanel.SetKeyCountInstant(keyCount);
    }
    public static void CloseBossKeyPanel()
    {
        if (Instance == null) return;

        Instance._bossKeyPanel.gameObject.SetActive(false);
    }
    public static void AddBossKey()
    {
        if (Instance == null) return;

        Instance._bossKeyPanel.AddKey();
    }

    // skill
    public static void OpenSkillObtainPanel(SkillObtainPanel.SkillInfo info)
    {
        if (Instance == null) return;

        Instance._skillObtainPanel.Open(info);
        Instance.StartCoroutine(Instance.CloseSkillObtainPanel(Instance._skillObtainPanelDuration));
        Instance._statusUI.alpha = 0;
    }

    public static void OpenDoubleJumpSkillObtainPanel()
    {
        int skillPieceCount = PersistentDataManager.GetByGlobal<int>("SkillPiece");

            var info = new SkillObtainPanel.SkillInfo();

            info.Icon = PersistentDataManager.SkillOrderData[skillPieceCount / 3].UnlockImage;
            info.MainText = PersistentDataManager.SkillOrderData[skillPieceCount / 3].Name;
            info.DetailText = "";

            CustomKeyCode keyCode = InputManager.Instance.DefaultInputSetter.GetKeyCode(PersistentDataManager.SkillOrderData[skillPieceCount / 3 - 1].Key);

            if (keyCode != null)
            {
                info.DetailText += keyCode.KeyCode.ToString();
            }

            info.DetailText += PersistentDataManager.SkillOrderData[skillPieceCount / 3].DetailText;

            OpenSkillObtainPanel(info);
        
    }

    private IEnumerator CloseSkillObtainPanel(float duration)
    {
        if (Instance == null) yield break;

        yield return new WaitForSeconds(duration);
        Instance._statusUI.alpha = 1;
        _skillObtainPanel.Close();
    }

    // item
    public static void OpenItemObtainPanel(ItemObtainPanel.ItemObtainInfo info)
    {
        if (Instance == null) return;

        Instance._itemObtainPanel.Open(info);
        Instance.StartCoroutine(Instance.CloseItemObtainPanel(Instance._itemObtainPanelDuration));
        Instance._statusUI.alpha = 0;
    }
    private IEnumerator CloseItemObtainPanel(float duration)
    {
        if (Instance == null) yield break;

        yield return new WaitForSeconds(duration);
        Instance._statusUI.alpha = 1;
        _itemObtainPanel.Close();
    }

    // letter box
    public static void OpenLetterbox(bool instant = false)
    {
        if (Instance == null) return;

        Instance._letterBox.Open(instant);
        Instance._statusUI.alpha = 0;
    }
    public static void CloseLetterbox()
    {
        if (Instance == null) return;

        Instance._letterBox.Close();
        Instance._statusUI.alpha = 1;
    }

    // option view
    public static void OpenOptionView()
    {
        if (Instance == null) return;

        Instance._optionView.TogglePanel();
    }

    // dead panel
    public static void OpenDeadPanel(bool respawnHereScene = false)
    {
        if (Instance == null) return;

        Instance._deadPanel.Open(respawnHereScene);
    }

    // scene name text
    public static void SetSceneNameText(string sceneName)
    {
        if (Instance == null) return;

        // Instance._sceneNameText.text = "Scene Name: " + sceneName;
    }

    // debug text
    public static void SetDebugText(string text)
    {
        if (Instance == null) return;

        // Instance._debugText.text = text;
    }
}
