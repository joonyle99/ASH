using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private void Start()
    {
        _proceedText.gameObject.SetActive(false);
        StartCoroutine(PlayScripts());
    }
    public void Update()
    {
        // CHEAT: F12 키를 누르면 프롤로그 스킵
        if (Input.GetKeyDown(KeyCode.F12))
        {
            StopAllCoroutines();
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        PersistentDataManager.ClearPersistentData();

        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToPlayableScene("1-1", "Enter 1-1");
    }
    private IEnumerator PlayScripts()
    {
        foreach(var script in _scripts)
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
            while(eTime < _fadeOutDuration)
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

        yield return StartGame();
    }
    private IEnumerator PlayScript(Script script)
    {
        for(int i=0; i<_texts.Length; i++)
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
        for(int i=0; i< script.Lines.Count; i++)
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
}
