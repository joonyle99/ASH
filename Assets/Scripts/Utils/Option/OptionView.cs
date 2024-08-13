using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

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
        InitialVolumeSetting();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Resume();
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
        _isPause = true;

        Time.timeScale = 0f;

        _optionPanel.gameObject.SetActive(true);

        InputManager.Instance.ChangeToStayStillSetter();
    }

    public void Resume()
    {
        _isPause = false;

        Time.timeScale = 1f;

        _optionPanel.gameObject.SetActive(false);

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

    // checkPoint
    public void MoveToCheckPoint()
    {
        Resume();

        // 새로운 씬이 아닌 '마지막 체크 포인트'에서 재시작 하도록 수정
        SceneContext.Current.Player.TriggerInstantRespawn(0f);
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
}
