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

    [Space]

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _bgmValue;
    [SerializeField] private TextMeshProUGUI _sfxValue;

    [Space]

    [Header("Resolution")]
    [SerializeField] private int _width = 1600;
    [SerializeField] private int _height = 900;
    [SerializeField] private bool _isFullScreen = true;

    private bool _isPause = false;
    public bool IsPause => _isPause;

    private void Awake()
    {
        // subscribe slider event to control volume
        _bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBgmVolume);
        _bgmSlider.onValueChanged.AddListener(SetBgmValue);

        _sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSfxVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxValue);
    }
    private void Start()
    {
        StartCoroutine(SetupVolumeCoroutine());
    }

    private void Update()
    {
        if (InputManager.Instance.State.EscapeKey.KeyDown)
        {
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

        InputManager.Instance.ChangeToDefaultSetter();
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

    public void ToggleResolution()
    {
        _isFullScreen = !_isFullScreen;

        _width = _isFullScreen ? Screen.width : 1600;
        _height = _isFullScreen ? Screen.height : 900;

        Screen.SetResolution(_width, _height, _isFullScreen);
    }

    private IEnumerator SetupVolumeCoroutine()
    {
        yield return null;

        InitialVolumeSetting();
    }
}
