using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 다이얼로그를 출력하기 위한 뷰 UI
/// </summary>
public class DialogueView : HappyTools.SingletonBehaviour<DialogueView>
{
    [Header("Dialogue View")]
    [Space]

    [SerializeField] private Image _dialoguePanel;
    [SerializeField] private Image _skipUI;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _nameText;

    // TODO: 퀘스트를 위해 수락 / 거절 버튼도 사용한다면 추가한다

    private TextShaker _textShaker;

    private DialogueLine _currentLine;
    private Coroutine _currentLineCoroutine;

    public bool IsCurrentLineOver { get; private set; }
    public bool IsPanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    public void OpenPanel()
    {
        _dialogueText.text = "";
        _skipUI.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogueText.GetComponent<TextShaker>();
    }
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
    public void FastForward()
    {
        StopCoroutine(_currentLineCoroutine);
        CleanUpOnSingleLineOver();
    }
    public void StartSingleLine(DialogueLine line)
    {
        _currentLine = line;
        IsCurrentLineOver = false;
        _skipUI.gameObject.SetActive(false);
        _dialogueText.alpha = 1;

        _nameText.text = line.Speaker;
        //Set shake
        if (line.ShakeParams == TextShakeParams.None)
            _textShaker.StopShake();
        else
        {
            _textShaker.shakeParams = line.ShakeParams;
            _textShaker.StartShake();
        }

        _currentLineCoroutine = StartCoroutine(SingleLineCoroutine());
    }
    public IEnumerator ClearTextCoroutine(float duration)
    {
        float eTime = 0;
        while (eTime < duration)
        {
            _dialogueText.alpha = 1 - (eTime / duration) * (eTime / duration);
            yield return null;
            eTime += Time.deltaTime;
        }
    }
    private IEnumerator SingleLineCoroutine()
    {
        StringBuilder stringBuilder = new StringBuilder(_currentLine.Text.Length);
        int textIndex = 0;

        while (true)
        {
            if (_currentLine.Text[textIndex] == '<')
            {
                int to = _currentLine.Text.IndexOf('>', textIndex);
                stringBuilder.Append(_currentLine.Text.Substring(textIndex, to + 1 - textIndex));
                textIndex = to;
            }
            else
            {
                stringBuilder.Append(_currentLine.Text[textIndex]);
                _dialogueText.text = stringBuilder.ToString();
                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Script" + Random.Range(1, 6).ToString());
            }

            textIndex++;
            if (textIndex == _currentLine.Text.Length)
                break;

            yield return new WaitForSeconds(_currentLine.CharShowInterval);
        }
        CleanUpOnSingleLineOver();
    }
    private void CleanUpOnSingleLineOver()
    {
        IsCurrentLineOver = true;
        _dialogueText.text = _currentLine.Text;
        _skipUI.gameObject.SetActive(true);
    }
}
