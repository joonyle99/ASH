using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] TextMeshProUGUI _bgmText;
    [SerializeField] TextMeshProUGUI _sfxText;

    private void Awake()
    {
        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        SetBGMText();
        SetSFXVolume();
    }

    public void SetBGMVolume(float volume)
    {
        if (volume == 0)
        {
            _audioMixer.SetFloat("BGM", -80);
        }
        else
        {
            _audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        SetBGMText();
    }

    public void SetSFXVolume(float volume)
    {
        if(volume == 0)
        {
            _audioMixer.SetFloat("SFX", -80);
        } else
        {
            _audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        SetSFXVolume();
    }

    public void SetBGMText()
    {
        int bgmVolume = Mathf.FloorToInt(_bgmSlider.value * 100);

        _bgmText.text = bgmVolume.ToString();
    }

    public void SetSFXVolume()
    {
        int sfxVolume = Mathf.FloorToInt(_sfxSlider.value * 100);

        _sfxText.text = sfxVolume.ToString();
    }

    public void OpenTitleScene()
    {
        StartCoroutine(OpenTitleSceneCoroutine());
    }

    IEnumerator OpenTitleSceneCoroutine()
    {
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToScene("TitleScene");
    }
}
