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
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private TextMeshProUGUI _speaker;
    [SerializeField] private Image _skipUI;
    [SerializeField] private Image _responsePanel;

    private TextShaker _textShaker;

    private DialogueSegment _currentSegment;
    private Coroutine _currentSegmentCoroutine;

    public bool IsCurrentSegmentOver { get; private set; }
    public bool IsDialoguePanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    /// <summary>
    /// 다이얼로그 뷰 UI 초기화 및 패널 열기
    /// </summary>
    public void OpenPanel()
    {
        _dialogue.text = "";
        _speaker.text = "";
        _skipUI.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _textShaker = _dialogue.GetComponent<TextShaker>();
    } 
    /// <summary>
    /// 퀘스트 응답 패널 열기
    /// </summary>
    public void OpenResponsePanel()
    {
        _skipUI.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(true);
    }
    /// <summary>
    /// 퀘스트 응답 패널에 퀘스트 전달
    /// </summary>
    public void SendQuestDataToResponsePanel(QuestData questData, out QuestResponse response)
    {
        response = _responsePanel.GetComponent<QuestResponse>();
        response.ReceiveQuestData(questData);
    }
    /// <summary>
    /// 다이얼로그 뷰 UI 패널 닫기
    /// </summary>
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
    /// <summary>
    /// 다이얼로그 세그먼트 빠르게 넘어가기
    /// </summary>
    public void FastForward()
    {
        StopCoroutine(_currentSegmentCoroutine);
        CleanUpOnSegmentOver();
    }
    /// <summary>
    /// 다이얼로그 세그먼트 종료 처리
    /// </summary>
    private void CleanUpOnSegmentOver()
    {
        IsCurrentSegmentOver = true;
        _dialogue.text = _currentSegment.Text;
        _skipUI.gameObject.SetActive(true);     // 스킵 UI 활성화
    }
    /// <summary>
    /// 다음 다이얼로그 세그먼트 시작
    /// </summary>
    /// <param name="segment"></param>
    public void StartNextSegment(DialogueSegment segment)
    {
        IsCurrentSegmentOver = false;

        // 세그먼트 초기화
        _dialogue.text = "";
        _dialogue.alpha = 1f;
        _skipUI.gameObject.SetActive(false);

        // 세그먼트 설정
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

        // 다이얼로그 세그먼트 프로세스 코루틴 시작 및 저장
        _currentSegmentCoroutine = StartCoroutine(SegmentCoroutine());
    }
    /// <summary>
    /// 다이얼로그 세그먼트 서서히 사라짐
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public IEnumerator ClearTextCoroutine(float duration)
    {
        float eTime = 0;
        while (eTime < duration)
        {
            yield return null;
            eTime += Time.deltaTime;
            _dialogue.alpha = 1f - (eTime / duration) * (eTime / duration);      // easing function: x^2
        }
    }
    /// <summary>
    /// 다이얼로그 세그먼트를 한 글자씩 출력하는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SegmentCoroutine()
    {
        // 세그먼트의 대사만큼 StringBuilder 생성
        StringBuilder stringBuilder = new StringBuilder(_currentSegment.Text.Length);

        int textIndex = 0;

        // 세그먼트의 대사를 한 글자씩 출력
        while (true)
        {
            // TODO: 사실 잘 모르겠다. 왜 stringBuilder에 추가하는지
            // '<'와 '>' 사이의 문자열을 추출해 stringBuilder에 추가
            // 이는 텍스트 태그를 파싱하기 위함
            if (_currentSegment.Text[textIndex] == '<')
            {
                int to = _currentSegment.Text.IndexOf('>', textIndex);
                string textTag = _currentSegment.Text.Substring(textIndex, to + 1 - textIndex);
                stringBuilder.Append(textTag);
                textIndex = to;
            }
            // 일반 텍스트를 stringBuilder에 추가
            else
            {
                stringBuilder.Append(_currentSegment.Text[textIndex]);
                _dialogue.text = stringBuilder.ToString();

                // 글자 출력 사운드 재생
                SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Script" + Random.Range(1, 6).ToString());
            }

            // 다음 글자로 이동
            textIndex++;

            // 세그먼트의 대사를 모두 출력했을 경우 종료
            if (textIndex == _currentSegment.Text.Length)
                break;

            yield return new WaitForSeconds(_currentSegment.CharShowInterval);
        }

        // 세그먼트 마무리 단계
        CleanUpOnSegmentOver();
    }
}
