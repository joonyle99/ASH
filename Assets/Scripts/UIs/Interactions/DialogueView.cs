using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    [SerializeField] private Image _skipUI;
    [SerializeField] private Image _responsePanel;
    [Tooltip("�������� ����(����, ����) ���� �г�")]
    [SerializeField] private ResponsePanel _responsePanel02;

    private TextShaker _textShaker;

    private DialogueSegment _currentSegment;
    private Coroutine _currentSegmentCoroutine;

    public bool IsCurrentSegmentOver { get; private set; }
    public bool IsDialoguePanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    /// <summary>
    /// ���̾�α� �� UI �ʱ�ȭ �� �г� ����
    /// </summary>
    public void OpenPanel()
    {
        _dialogue.text = "";
        _speaker.text = "";
        _skipUI.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(false);
        _responsePanel02.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogue.GetComponent<TextShaker>();
    } 
    /// <summary>
    /// ����Ʈ ���� �г� ����
    /// </summary>
    public void OpenResponsePanel()
    {
        _skipUI.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(true);
    }
    /// <summary>
    /// ����Ʈ ���� �г� ����
    /// </summary>
    public void OpenResponsePanel02(List<ResponseFunctionContainer> responseFunctions)
    {
        _skipUI.gameObject.SetActive(false);

        _responsePanel02.gameObject.SetActive(true);
        if(responseFunctions != null)
        {
            for (int i = 0; i < responseFunctions.Count; i++)
            {
                ResponseFunctionContainer responseFunctionContainer = responseFunctions[i];
                _responsePanel02.BindActionOnClicked(responseFunctionContainer.buttonType, responseFunctionContainer.action);
            }
        }
    }

    /// <summary>
    /// ����Ʈ ���� �гο� ����Ʈ ����
    /// </summary>
    public void SendQuestDataToResponsePanel(QuestData questData, out QuestResponse response)
    {
        response = _responsePanel.GetComponent<QuestResponse>();
        response.ReceiveQuestData(questData);
    }
    /// <summary>
    /// ���̾�α� �� UI �г� �ݱ�
    /// </summary>
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// ���̾�α� ���׸�Ʈ ������ �Ѿ��
    /// </summary>
    public void FastForward()
    {
        StopCoroutine(_currentSegmentCoroutine);
        CleanUpOnSegmentOver();
    }
    /// <summary>
    /// ���̾�α� ���׸�Ʈ ���� ó��
    /// </summary>
    private void CleanUpOnSegmentOver()
    {
        IsCurrentSegmentOver = true;
        _dialogue.text = _currentSegment.Text;
        _skipUI.gameObject.SetActive(true);     // ��ŵ UI Ȱ��ȭ
    }
    /// <summary>
    /// ���� ���̾�α� ���׸�Ʈ ����
    /// </summary>
    /// <param name="segment"></param>
    public void StartNextSegment(DialogueSegment segment)
    {
        IsCurrentSegmentOver = false;

        // ���׸�Ʈ �ʱ�ȭ
        _dialogue.text = "";
        _dialogue.alpha = 1f;
        _skipUI.gameObject.SetActive(false);

        // ���׸�Ʈ ����
        _currentSegment = segment;
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
    /// <summary>
    /// ���̾�α� ���׸�Ʈ ������ �����
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
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
    /// <summary>
    /// ���̾�α� ���׸�Ʈ�� �� ���ھ� ����ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator SegmentCoroutine()
    {
        // �� ������ ���� ����
        yield return null;

        // ���׸�Ʈ�� ��縸ŭ StringBuilder ����
        StringBuilder stringBuilder = new StringBuilder(_currentSegment.Text.Length);

        int textIndex = 0;

        // ���׸�Ʈ�� ��縦 �� ���ھ� ���
        while (true)
        {
            // �̴� �±׸� �Ľ��ϱ� ����
            if (_currentSegment.Text[textIndex] == '<')
            {
                int to = _currentSegment.Text.IndexOf('>', textIndex);
                string textTag = _currentSegment.Text.Substring(textIndex, to + 1 - textIndex);
                stringBuilder.Append(textTag);
                textIndex = to;
            }
            // �Ϲ� �ؽ�Ʈ�� stringBuilder�� �߰�
            else
            {
                stringBuilder.Append(_currentSegment.Text[textIndex]);
                _dialogue.text = stringBuilder.ToString();

                // ���� ��� ���� ���
                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Script" + Random.Range(1, 6).ToString());
            }

            // ���� ���ڷ� �̵�
            textIndex++;

            // ���׸�Ʈ�� ��縦 ��� ������� ��� ����, �Ǵ� ��������
            if (textIndex == _currentSegment.Text.Length || DialogueController.Instance.IsShutdowned)
                break;

            yield return new WaitForSeconds(_currentSegment.CharShowInterval);
        }

        // ���׸�Ʈ ������ �ܰ�
        CleanUpOnSegmentOver();
    }
}
