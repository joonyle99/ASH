using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// ���̾�α׸� ����ϱ� ���� �� UI
/// </summary>
public class DialogueView : MonoBehaviour
{
    [Header("Dialogue View")]
    [Space]

    [SerializeField] private Image _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private TextMeshProUGUI _speaker;
    [SerializeField] private Image _indicator;

    [Tooltip("�������� ����(����, ����) ���� �г�")]
    [SerializeField] private ResponsePanel _responsePanel;
    public ResponsePanel ResponsePanel => _responsePanel;

    private TextShaker _textShaker;

    private DialogueSegment _currentSegment;
    private string _exceptTimeSegmentText;          // '[3]'�� ���� ��� �ð��� ������ ���׸�Ʈ �ؽ�Ʈ

    private Coroutine _currentSegmentCoroutine;

    public bool IsCurrentSegmentOver { get; private set; }
    public bool IsDialoguePanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    /// <summary> ���̾�α� �� UI �ʱ�ȭ �� �г� ���� </summary>
    public void OpenPanel()
    {
        _dialogue.text = "";
        _speaker.text = "";
        _indicator.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogue.GetComponent<TextShaker>();
    }
    /// <summary> ���̾�α� �� UI �г� �ݱ� </summary>
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }

    /// <summary> ���� �г� ���� </summary>
    public void OpenResponsePanel(List<ResponseContainer> responseFunctions)
    {
        _indicator.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(true);

        if (responseFunctions != null)
        {
            for (int i = 0; i < responseFunctions.Count; i++)
            {
                var responseFunctionContainer = responseFunctions[i];
                _responsePanel.BindActionOnClicked(responseFunctionContainer.buttonType, responseFunctionContainer.action);
            }
        }
    }

    /// <summary> ���� ���̾�α� ���׸�Ʈ ���� </summary>
    public void StartNextSegment(DialogueSegment segment)
    {
        IsCurrentSegmentOver = false;

        // ���׸�Ʈ �ʱ�ȭ
        _dialogue.text = "";
        _dialogue.alpha = 1f;
        _indicator.gameObject.SetActive(false);

        // ���׸�Ʈ ����
        _currentSegment = segment;
        _exceptTimeSegmentText = ExtractTextWithoutTime(segment.Text);
        _speaker.text = segment.Speaker;

        // Set shake
        if (segment.ShakeParams == TextShakeParams.None)
            _textShaker.StopShake();
        else
        {
            _textShaker.shakeParams = segment.ShakeParams;
            _textShaker.StartShake();
        }

        // ���̾�α� ���׸�Ʈ ���μ��� �ڷ�ƾ ���� �� ����
        _currentSegmentCoroutine = StartCoroutine(SegmentCoroutine());
    }
    /// <summary> ���̾�α� ���׸�Ʈ�� �� ���ھ� ����ϴ� �ڷ�ƾ </summary>
    private IEnumerator SegmentCoroutine()
    {
        // �� ������ ���� ����
        yield return null;

        // ���׸�Ʈ�� ��� ũ�⸸ŭ StringBuilder ����
        StringBuilder stringBuilder = new StringBuilder(_currentSegment.Text.Length);

        int textIndex = 0;

        // ���׸�Ʈ�� ��縦 �� ���ھ� ��ȸ
        while (true)
        {
            // �̴� �±׸� �Ľ��ϱ� ����
            if (_currentSegment.Text[textIndex] == '<')
            {
                int from = textIndex;
                int to = _currentSegment.Text.IndexOf('>', from);
                string textTag = _currentSegment.Text.Substring(from, to - from + 1);       // from�� ������ length��ŭ substring

                stringBuilder.Append(textTag);

                textIndex = to;
            }
            // �ؽ�Ʈ õõ�� ���
            else if (_currentSegment.Text[textIndex] == '[')
            {
                int from = textIndex;
                int to = _currentSegment.Text.IndexOf(']', from);
                string textTime = _currentSegment.Text.Substring(from + 1, to - from - 1);

                if (float.TryParse(textTime, out var waitTime))
                {
                    yield return new WaitForSeconds(waitTime);
                }
                else
                {
                    Debug.LogError($"textTime format is invalid\n{Environment.StackTrace}");
                }

                textIndex = to;
            }
            // �Ϲ� �ؽ�Ʈ�� stringBuilder�� �߰�
            else
            {
                stringBuilder.Append(_currentSegment.Text[textIndex]);
                _dialogue.text = stringBuilder.ToString();      // ��������� stringBuilder�� �ؽ�Ʈ�� ���

                // ���� ��� ���� ���
                SoundManager.Instance.PlayCommonSFX("SE_UI_Script" + UnityEngine.Random.Range(1, 6).ToString());
            }

            // ���� ���ڷ� �̵�
            textIndex++;

            // ���׸�Ʈ�� ��縦 ��� ������� ��� ����, �Ǵ� ��������
            if (textIndex == _currentSegment.Text.Length)
                break;

            yield return new WaitForSeconds(_currentSegment.CharShowInterval);
        }

        // ���׸�Ʈ ������ �ܰ�
        CleanUpOnSegmentOver();
    }
    /// <summary> ���̾�α� ���׸�Ʈ ������ ����� </summary>
    public IEnumerator ClearTextCoroutine(float duration)
    {
        float eTime = 0;
        while (eTime < duration)
        {
            _dialogue.alpha = 1f - (eTime / duration) * (eTime / duration);      // easing function: x^2
            yield return null;
            eTime += Time.deltaTime;
        }
    }
    /// <summary> ���̾�α� ���׸�Ʈ���� [Time]�� ���� </summary>
    private string ExtractTextWithoutTime(string originText)
    {
        StringBuilder result = new StringBuilder(originText.Length);

        for (int i = 0; i < originText.Length; i++)
        {
            if (originText[i] == '[')
            {
                var from = i;
                var to = originText.IndexOf(']', from);

                // ��ã�� ��� ������ �߻��ϵ��� �ϰ�, ���ȣ�� ���ڷ� �߰��Ѵ�
                if (to == -1)
                {
                    Debug.LogError($"The pair in '[' does not exist");
                    result.Append(originText, i, originText.Length - i);
                    break;
                }

                i = to;
            }
            else
            {
                result.Append(originText[i]);
            }
        }

        return result.ToString();
    }
    /// <summary> ���̾�α� ���׸�Ʈ ������ �Ѿ�� </summary>
    public void FastForward()
    {
        StopCoroutine(_currentSegmentCoroutine);
        CleanUpOnSegmentOver();
    }
    /// <summary> ���̾�α� ���׸�Ʈ ���� ó�� </summary>
    public void CleanUpOnSegmentOver()
    {
        IsCurrentSegmentOver = true;
        _dialogue.text = _exceptTimeSegmentText;
        _indicator.gameObject.SetActive(true);
    }

    public void SkipDialogue()
    {
        if(DialogueController.Instance)
            DialogueController.Instance.ShutdownDialogue();
    }
}
