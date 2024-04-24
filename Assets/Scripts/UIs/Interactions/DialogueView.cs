using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ���̾�α׸� ����ϱ� ���� �� UI
/// </summary>
public class DialogueView : HappyTools.SingletonBehaviour<DialogueView>
{
    [Header("Dialogue View")]
    [Space]

    [SerializeField] private Image _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private TextMeshProUGUI _speaker;
    [SerializeField] private Image _skipUI;

    private TextShaker _textShaker;

    private DialogueSegment currentSegment;
    private Coroutine _currentsegmentCoroutine;

    public bool IsCurrentSegmentOver { get; private set; }
    public bool IsPanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    /// <summary>
    /// ���̾�α� �� UI �ʱ�ȭ �� �г� ����
    /// </summary>
    public void OpenPanel()
    {
        _dialogue.text = "";
        _speaker.text = "";
        _skipUI.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogue.GetComponent<TextShaker>();
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
        StopCoroutine(_currentsegmentCoroutine);
        CleanUpOnSegmentOver();
    }
    /// <summary>
    /// ���̾�α� ���׸�Ʈ ���� ó��
    /// </summary>
    private void CleanUpOnSegmentOver()
    {
        IsCurrentSegmentOver = true;
        _dialogue.text = currentSegment.Text;
        _skipUI.gameObject.SetActive(true);     // ��ŵ UI Ȱ��ȭ
    }
    /// <summary>
    /// ���� ���̾�α� ���׸�Ʈ ����
    /// </summary>
    /// <param name="segment"></param>
    public void StartNextSegment(DialogueSegment segment)
    {
        // ���׸�Ʈ ����
        currentSegment = segment;
        IsCurrentSegmentOver = false;
        _skipUI.gameObject.SetActive(false);
        _speaker.text = segment.Speaker;
        _dialogue.alpha = 1;

        // Set shake
        if (segment.ShakeParams == TextShakeParams.None)
            _textShaker.StopShake();
        else
        {
            _textShaker.shakeParams = segment.ShakeParams;
            _textShaker.StartShake();
        }

        // ���̾�α� ���׸�Ʈ ���μ��� �ڷ�ƾ ���� �� ����
        _currentsegmentCoroutine = StartCoroutine(SegmentCoroutine());
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
            _dialogue.alpha = 1 - (eTime / duration) * (eTime / duration);
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
        // ���׸�Ʈ�� ��縸ŭ StringBuilder ����
        StringBuilder stringBuilder = new StringBuilder(currentSegment.Text.Length);

        int textIndex = 0;

        // ���׸�Ʈ�� ��縦 �� ���ھ� ���
        while (true)
        {
            // TODO: ��� �� �𸣰ڴ�. �� stringBuilder�� �߰��ϴ���
            // '<'�� '>' ������ ���ڿ��� ������ stringBuilder�� �߰�
            // �̴� �ؽ�Ʈ �±׸� �Ľ��ϱ� ����
            if (currentSegment.Text[textIndex] == '<')
            {
                int to = currentSegment.Text.IndexOf('>', textIndex);
                string textTag = currentSegment.Text.Substring(textIndex, to + 1 - textIndex);
                stringBuilder.Append(textTag);
                textIndex = to;
            }
            // �Ϲ� �ؽ�Ʈ�� stringBuilder�� �߰�
            else
            {
                stringBuilder.Append(currentSegment.Text[textIndex]);
                _dialogue.text = stringBuilder.ToString();

                // ���� ��� ���� ���
                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Script" + Random.Range(1, 6).ToString());
            }

            // ���� ���ڷ� �̵�
            textIndex++;

            // ���׸�Ʈ�� ��縦 ��� ������� ��� ����
            if (textIndex == currentSegment.Text.Length)
                break;

            yield return new WaitForSeconds(currentSegment.CharShowInterval);
        }

        // ���׸�Ʈ ������ �ܰ�
        CleanUpOnSegmentOver();
    }
}
