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

    private void Awake()
    {
        // subscribe slider event to control volume
        _bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }
    private void Start()
    {
        // set slider value
        SetBgmValue();
        SetSfxValue();
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

    public void SetBgmVolume(float volume)
    {
        if (volume < 0.01f)
        {
            _audioMixer.SetFloat("BGM", -80);
        }
        else
        {
            _audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        SetBgmValue();
    }
    public void SetSfxVolume(float volume)
    {
        if (volume == 0)
        {
            _audioMixer.SetFloat("SFX", -80);
        }
        else
        {
            _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        SetSfxValue();
    }

    public void SetBgmValue()
    {
        int bgmVolume = Mathf.FloorToInt(_bgmSlider.value * 100f);

        _bgmValue.text = bgmVolume.ToString();
    }
    public void SetSfxValue()
    {
        int sfxVolume = Mathf.FloorToInt(_sfxSlider.value * 100f);

        _sfxValue.text = sfxVolume.ToString();
    }

    public void SwitchToWindowedMode(bool isToggleOn)
    {
        // if (UnityEngine.Screen.fullScreenMode == FullScreenMode.Windowed) return;

        Debug.Log("â ���");

        UnityEngine.Screen.fullScreenMode = FullScreenMode.Windowed;

        UnityEngine.Screen.SetResolution(1280, 720, false);
    }
    public void SwitchToFullScreenMode(bool isToggleOn)
    {
        // if (UnityEngine.Screen.fullScreenMode == FullScreenMode.FullScreenWindow) return;

        Debug.Log("��ü ���");

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
        Debug.Log("������ ����� �մϴ�.");

        StartCoroutine(ReStartCoroutine());
    }
    private IEnumerator ReStartCoroutine()
    {
        // TODO: Time.TimeScale == 0f �� ��쿡 ���� �Ѿ�� �ʴ´�.
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToPlayableScene(SceneManager.GetActiveScene().name, SceneContext.Current.EntrancePassage.PassageName);

        Debug.Log("End ReStart Coroutine");
    }
}
