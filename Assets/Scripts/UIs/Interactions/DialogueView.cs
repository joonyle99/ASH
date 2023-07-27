using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueView : MonoBehaviour
{
    [SerializeField] Image _dialoguePanel;
    [SerializeField] Image _skipUI;
    [SerializeField] TextMeshProUGUI _dialogueText;

    TextShaker _textShaker;

    bool _fastForward = false;
    public void OpenPanel()
    {
        _skipUI.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogueText.GetComponent<TextShaker>();
    }
    public IEnumerator FadeOutCoroutine(float duration)
    {
        float eTime = 0;
        while (eTime < duration)
        {
            _dialogueText.alpha = 1 - (eTime / duration) * (eTime / duration);
            yield return null;
            eTime += Time.deltaTime;
        }
    }
    public IEnumerator StartScriptCoroutine(DialogueScriptInfo script)
    {
        _skipUI.gameObject.SetActive(false);
        _dialogueText.alpha = 1;
        float charInterval = 1.0f / script.Speed;
        if (script.Shake.Speed > 0)
        {
            _textShaker.AngleMultiplier = script.Shake.RotationPower;
            _textShaker.MoveMultiplier = script.Shake.MovePower;
            _textShaker.SpeedMultiplier = script.Shake.Speed;
            _textShaker.StartShake();
        }
        else
            _textShaker.StopShake();
        string shownScript = "";
        int index = 0;
        _fastForward = false;
        while (true)
        {
            if (_fastForward)
            {

                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
                break;
            }
            if (script.Text[index] == '<')
            {
                index = script.Text.IndexOf('>', index);
                shownScript = script.Text.Substring(0, index+1);
            }
            else
            {
                shownScript += script.Text[index];
                _dialogueText.text = shownScript;
                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Script" + Random.Range(1, 6).ToString());
            }

            index++;
            if (index == script.Text.Length)
                break;

            float eTime = 0f;
            while (eTime < charInterval)
            {
                eTime += Time.deltaTime;
                yield return null;
                if (_fastForward)
                    break;
            }
        }
        _fastForward = false;
        _dialogueText.text = script.Text;
        _skipUI.gameObject.SetActive(true);
        yield return null;
    }
    private void Update()
    {
        if (InputManager.InteractionKeyDown)
            _fastForward = true;
    }
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
}
