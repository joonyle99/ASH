using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    [System.Serializable]
    public class Script
    {
        public List<string> Lines;
    }

    [SerializeField] float _fadeInDuration = 0.3f;
    [SerializeField] float _fadeOutDuration = 0.3f;
    [SerializeField] float _lineWaitDuration = 2f;
    [SerializeField] float _pageWaitDuration = 2f;

    [Space]

    [SerializeField] TextMeshProUGUI _proceedText;
    [SerializeField] TextMeshProUGUI[] _texts;
    [SerializeField] List<Script> _scripts;

    [Space]

    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] VideoClip _videoEn;
    [SerializeField] VideoClip _videoJp;
    [SerializeField] VideoClip _videoKo;

    [Space]
    
    [SerializeField] Button _skipButton;
    [SerializeField] TextMeshProUGUI _skipText;

    private void Start()
    {
        _proceedText.gameObject.SetActive(false);

        _skipButton.gameObject.SetActive(false);

        PlayPrologue();

        //StartCoroutine(PlayScripts());

        _videoPlayer.loopPointReached -= StartGame;
        _videoPlayer.loopPointReached += StartGame;
    }
    private void OnDestroy()
    {
        _videoPlayer.loopPointReached -= StartGame;
    }
    public void Update()
    {
        // CHEAT: F12 키를 누르면 프롤로그 스킵
        if (Input.GetKeyDown(KeyCode.F12) && GameSceneManager.Instance.CheatMode == true)
        {
            SkipPrologue();
        }
    }

    private void PlayPrologue()
    {
        switch (DialogueDataManager.Instance.GetLanguageCode())
        {
            case LanguageCode.KOREAN:
                _videoPlayer.clip = _videoKo;
                break;
            case LanguageCode.ENGLISH:
                _videoPlayer.clip = _videoEn;
                break;
            case LanguageCode.JAPANESE:
                _videoPlayer.clip = _videoJp;
                break;
        }

        _videoPlayer.Play();
        _skipText.text = UITranslator.GetLocalizedString("ui_skip");

        StartCoroutine(FadeInButtonComponents(_skipButton, 2f));
    }

    private void StartGame(UnityEngine.Video.VideoPlayer _videoPlayer = null)
    {
        // 데이터 초기화
        PersistentDataManager.ClearPersistentData();
        PersistentDataManager.ClearSavedPersistentData();
        QuestController.Instance.InitializeQuestData();

        SceneChangeManager.Instance.ChangeToPlayableScene("1-1", "Enter 1-1");
    }

    private IEnumerator PlayScripts()
    {
        float scriptDelayTime = 3f;

        yield return new WaitForSeconds(scriptDelayTime);

        foreach (var script in _scripts)
        {
            yield return PlayScript(script);
            yield return FadeInProceedText();
            yield return new WaitUntil(() => Input.anyKeyDown);
            SoundManager.Instance.PlayCommonSFX("SE_UI_Button");

            Color color = _proceedText.color;
            color.a = 0;
            _proceedText.color = color;

            float eTime = 0f;
            color = _texts[0].color;
            while (eTime < _fadeOutDuration)
            {
                color.a = 1 - eTime / _fadeOutDuration;
                foreach (var text in _texts)
                    text.color = color;
                yield return null;
                eTime += Time.deltaTime;
            }
            color.a = 0;
            foreach (var text in _texts)
                text.color = color;
        }

        yield return null;
    }
    private IEnumerator PlayScript(Script script)
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            if (i < script.Lines.Count)
            {
                Color color = _texts[i].color;
                color.a = 0;
                _texts[i].color = color;
                _texts[i].gameObject.SetActive(true);
                _texts[i].text = script.Lines[i];
            }
            else
                _texts[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < script.Lines.Count; i++)
        {

            float eTime = 0f;
            var color = _texts[i].color;
            while (eTime < _fadeInDuration)
            {
                color.a = eTime / _fadeInDuration;
                _texts[i].color = color;
                yield return null;
                eTime += Time.deltaTime;
            }
            color.a = 1;
            _texts[i].color = color;
            yield return new WaitForSeconds(_lineWaitDuration);
        }
    }
    private IEnumerator FadeInProceedText()
    {
        Color color = _proceedText.color;
        color.a = 0;
        _proceedText.color = color;
        _proceedText.gameObject.SetActive(true);
        float eTime = 0f;
        while (eTime < _fadeInDuration)
        {
            color.a = eTime / _fadeInDuration;
            _proceedText.color = color;
            yield return null;
            eTime += Time.deltaTime;
        }
        color.a = 1;
        _proceedText.color = color;
    }
    
    public void SkipPrologue()
    {
        StopAllCoroutines();
        StartGame();
    }

    private IEnumerator FadeInButtonComponents(Button button, float fadeDuration)
    {
        // 10초 대기 후에 스킵 버튼 활성화
        yield return new WaitForSeconds(10f);

        // 버튼 활성화
        button.gameObject.SetActive(true);

        // 컴포넌트 가져오기
        Image image = button.GetComponent<Image>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

        // 초기 알파 설정
        Color imageColor = image.color;
        Color textColor = text.color;
        imageColor.a = 0f;
        textColor.a = 0f;
        image.color = imageColor;
        text.color = textColor;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = elapsed / fadeDuration;

            imageColor.a = alpha;
            textColor.a = alpha;
            image.color = imageColor;
            text.color = textColor;

            yield return null;
            elapsed += Time.deltaTime;
        }

        // 최종 알파 1로 설정
        imageColor.a = 1f;
        textColor.a = 1f;
        image.color = imageColor;
        text.color = textColor;
    }
}
