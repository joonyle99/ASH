using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OptionView : MonoBehaviour
{
    [Header("Option View")]
    [Space]

    [SerializeField] private Image _optionPanel;
    [SerializeField] private bool _isTitleScene = false;

    [Space]

    [Header("Audio")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _bgmValue;
    [SerializeField] private TextMeshProUGUI _sfxValue;

    [Space]

    [Header("Resolution")]
    [SerializeField] private GameObject Window;
    [SerializeField] private GameObject WindowChecked;
    [SerializeField] private GameObject FullScreen;
    [SerializeField] private GameObject FullScreenChecked;

    [SerializeField] private int _width = 1600;
    [SerializeField] private int _height = 900;
    [SerializeField] private bool _isFullScreen = true;
    public bool IsFullScreen
    {
        get => _isFullScreen;
        set
        {
            Window?.SetActive(value);
            WindowChecked?.SetActive(!value);

            FullScreen?.SetActive(!value);
            FullScreenChecked?.SetActive(value);
            _isFullScreen = value;
        }
    }

    [Header("Language")]
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private TextMeshProUGUI _language;
    [SerializeField] private TextMeshProUGUI _language2;

    [Header("External Reference")]
    [Space]
    [SerializeField] private KeySettingUIManager _keySettingUIManager;

    private bool _isPause = false;
    public bool IsPause => _isPause;

    private void Awake()
    {
        // subscribe slider event to control volume
        _bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
        _bgmSlider.onValueChanged.AddListener(SetBgmValue);

        _sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxValue);

        _dropdown.onValueChanged.AddListener((optionIdx) => ChangeLanguage(optionIdx));
    }
    private void Start()
    {
        //IsFullScreen = true;

        ApplyLanguageText();
        StartCoroutine(SetupVolumeCoroutine());

        _dropdown.value = (int)DialogueDataManager.Instance.GetLanguageCode();
    }

    private void Update()
    {
        if (InputManager.Instance.State.EscapeKey.KeyDown)
        {
            if(!_isTitleScene)
            {
                var PlayablePlayer = SceneContext.Current.PlayableSceneTransitionPlayer;
                if ((PlayablePlayer != null && PlayablePlayer.IsPlayable == false) ||
                    _keySettingUIManager.IsOpened() || SceneEffectManager.Instance.IsPlayingCutscene)
                {
                    return;
                }
            }

            TogglePanel();
        }
    }

    // option button
    public void TogglePanel()
    {
        if (_isPause)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (_isPause) return;

        _isPause = true;

        Time.timeScale = 0f;

        _optionPanel.gameObject.SetActive(true);

        SoundManager.Instance.PauseAllSound();
    }

    public void Resume()
    {
        if (!_isPause) return;

        _isPause = false;

        Time.timeScale = 1f;

        _optionPanel.gameObject.SetActive(false);

        DialogueController.Instance.ShutdownDialogue();

        SoundManager.Instance.UnPauseAllSound();
    }

    // volume setting
    private void InitialVolumeSetting()
    {
        SoundManager.Instance.InitialVolumeSetting();

        float bgmVolume = 1f;
        float sfxVolume = 1f;

        if (JsonDataManager.Has("BGMVolume"))
            bgmVolume = float.Parse(JsonDataManager.Instance.GlobalSaveData.saveDataGroup["BGMVolume"]);

        if (JsonDataManager.Has("SFXVolume"))
            sfxVolume = float.Parse(JsonDataManager.Instance.GlobalSaveData.saveDataGroup["SFXVolume"]);

        SetBgmValue(bgmVolume);
        SetSfxValue(sfxVolume);

        _bgmSlider.value = bgmVolume;
        _sfxSlider.value = sfxVolume;
    }

    public void SetBgmValue(float volume)
    {
        int bgmVolume = Mathf.FloorToInt(volume * 100f);

        _bgmValue.text = bgmVolume.ToString();
    }

    public void SetSfxValue(float volume)
    {
        int sfxVolume = Mathf.FloorToInt(volume * 100f);

        _sfxValue.text = sfxVolume.ToString();
    }

    // load
    public void Load()
    {
        Resume();

        PersistentDataManager.LoadToSavedData();
    }

    // tile
    public void GoToTitleScene()
    {
        Resume();

        SceneChangeManager.Instance.ChangeToNonPlayableScene("TitleScene");
    }

    public void SetFullScreenResolution()
    {
        IsFullScreen = true;
        Screen.SetResolution(Screen.width, Screen.height, true);
        Debug.Log("Set resolution to FullScreen");
    }

    public void SetWindowResolution()
    {
        IsFullScreen = false;
        Screen.SetResolution(1600, 900, false);
        Debug.Log("Set resolution to Window");
    }

    private IEnumerator SetupVolumeCoroutine()
    {
        yield return null;

        InitialVolumeSetting();
    }

    // language
    public void NextLanguage()
    {
        var currentLanguage = DialogueDataManager.Instance.GetLanguageCode();
        var nextLanguage = (LanguageCode)(Mathf.Clamp(((int)currentLanguage + 1) % ((int)LanguageCode.JAPANESE + 1), (int)LanguageCode.KOREAN, (int)LanguageCode.JAPANESE));
        ChangeLanguage(nextLanguage);
    }
    public void ChangeLanguage(int langIdx)
    {
        LanguageCode languageCode = LanguageCode.KOREAN; // Default to Korean
        switch (langIdx)
        {
            case 0:
                languageCode = LanguageCode.KOREAN;
                break;
            case 1:
                languageCode = LanguageCode.ENGLISH;
                break;
            case 2:
                languageCode = LanguageCode.JAPANESE;
                break;
        }
        DialogueDataManager.Instance.SetLanguageCode(languageCode);
        ApplyLanguageText();
    }
    public void ChangeLanguage(string lang)
    {
        LanguageCode languageCode = LanguageCode.KOREAN; // Default to Korean
        switch (lang)
        {
            case "ko":
                languageCode = LanguageCode.KOREAN;
                break;
            case "en":
                languageCode = LanguageCode.ENGLISH;
                break;
            case "ja":
                languageCode = LanguageCode.JAPANESE;
                break;
        }

        DialogueDataManager.Instance.SetLanguageCode(languageCode);
        ApplyLanguageText();
    }
    public void ChangeLanguage(LanguageCode languageCode)
    {
        DialogueDataManager.Instance.SetLanguageCode(languageCode);
        ApplyLanguageText();
    }

    private void ApplyLanguageText()
    {
        var languageText = UITranslator.GetLocalizedString("ui_language");

        _language.text = languageText;
        _language2.text = languageText;
    }
}
