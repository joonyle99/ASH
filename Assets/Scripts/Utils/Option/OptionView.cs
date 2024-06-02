using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionView : MonoBehaviour
{
    [Header("Option View")]
    [Space]

    [SerializeField] private Image _optionPanel;
    [SerializeField] private AudioMixer _audioMixer;

    [Space]

    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    [SerializeField] private TextMeshProUGUI _bgmValue;
    [SerializeField] private TextMeshProUGUI _sfxValue;

    [Space]

    [SerializeField] private Toggle _fullScreenToggle;
    [SerializeField] private Toggle _windowedToggle;

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
        // set slider value
        InitialSetting();
    }

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

    public void SwitchToWindowedMode(bool isToggleOn)
    {
        // if (UnityEngine.Screen.fullScreenMode == FullScreenMode.Windowed) return;

        Debug.Log("창 모드");

        UnityEngine.Screen.fullScreenMode = FullScreenMode.Windowed;

        UnityEngine.Screen.SetResolution(1280, 720, false);
    }
    public void SwitchToFullScreenMode(bool isToggleOn)
    {
        // if (UnityEngine.Screen.fullScreenMode == FullScreenMode.FullScreenWindow) return;

        Debug.Log("전체 모드");

        UnityEngine.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void Pause()
    {
        _isPause = true;

        Time.timeScale = 0f;

        _optionPanel.gameObject.SetActive(true);

        SceneContext.Current.Player.enabled = false;
    }
    public void Resume()
    {
        _isPause = false;

        Time.timeScale = 1f;

        _optionPanel.gameObject.SetActive(false);

        SceneContext.Current.Player.enabled = true;
    }

    public void ReStartGame()
    {
        StartCoroutine(ReStartCoroutine());
    }
    private IEnumerator ReStartCoroutine()
    {
        Resume();

        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();

        SceneChangeManager.Instance.ChangeToPlayableScene(SceneManager.GetActiveScene().name, SceneContext.Current.EntrancePassage.PassageName);
    }

    public void ApplyTestButton()
    {
        JsonDataManager.JsonSave();
    }

    private void InitialSetting()
    {
        float bgmVolume = 1f;
        float sfxVolume = 1f;

        if (JsonDataManager.Has("BGMVolume"))
            bgmVolume = float.Parse(JsonDataManager._globalSaveData.saveDataGroup["BGMVolume"]);

        if (JsonDataManager.Has("SFXVolume"))
            sfxVolume = float.Parse(JsonDataManager._globalSaveData.saveDataGroup["SFXVolume"]);

        SetBgmValue(bgmVolume);
        SetSfxValue(sfxVolume);

        _bgmSlider.value = bgmVolume;
        _sfxSlider.value = sfxVolume;
    }
}
