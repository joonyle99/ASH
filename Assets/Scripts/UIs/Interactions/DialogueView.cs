using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 다이얼로그를 출력하기 위한 뷰 UI
/// </summary>
public class DialogueView : MonoBehaviour
{
    [Header("Dialogue View")]
    [Space]

    [SerializeField] private Image _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogue;
    [SerializeField] private TextMeshProUGUI _speaker;
    [SerializeField] private Image _indicator;
    [SerializeField] private Button _skipButton;
    [SerializeField] private const int CountOfPlaySoundCharacter = 2;

    [Tooltip("스테이지 리셋(수락, 거절) 응답 패널")]
    [SerializeField] private ResponsePanel _responsePanel;
    public ResponsePanel ResponsePanel => _responsePanel;

    [SerializeField] private TMP_Text _skipText;

    private TextShaker _textShaker;

    private DialogueSegment _currentSegment;
    private string _exceptTimeSegmentText;          // '[3]'과 같은 대기 시간을 제외한 세그먼트 텍스트

    private Coroutine _currentSegmentCoroutine;

    public bool IsCurrentSegmentOver { get; private set; }
    public bool IsDialoguePanelActive => _dialoguePanel.gameObject.activeInHierarchy;

    /// <summary> 다이얼로그 뷰 UI 초기화 및 패널 열기 </summary>
    public void OpenPanel(bool canSkip = false)
    {
        _dialogue.text = "";
        _speaker.text = "";
        _indicator.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(false);
        _dialoguePanel.gameObject.SetActive(true);
        _skipButton.gameObject.SetActive(canSkip);
        _textShaker = _dialogue.GetComponent<TextShaker>();
        string dialogueKeyCode = InputManager.Instance.StayStillInputSetter.GetKeyCode("Dialogue").KeyCode.ToString();
        _skipText.text = dialogueKeyCode;
    }
    /// <summary> 다이얼로그 뷰 UI 패널 닫기 </summary>
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }

    /// <summary> 응답 패널 열기 </summary>
    public void OpenResponsePanel(List<ResponseContainer> responseFunctions)
    {
        _indicator.gameObject.SetActive(false);
        _responsePanel.gameObject.SetActive(true);

        if (responseFunctions != null)
        {
            for (int i = 0; i < responseFunctions.Count; i++)
            {
                _responsePanel.BindActionOnClicked(responseFunctions[i].buttonType, responseFunctions[i].action);
            }
        }
    }

    /// <summary> 다음 다이얼로그 세그먼트 시작 </summary>
    public void StartNextSegment(DialogueSegment segment)
    {
        IsCurrentSegmentOver = false;

        // 세그먼트 초기화
        _dialogue.text = "";
        _dialogue.alpha = 1f;
        _indicator.gameObject.SetActive(false);

        // 세그먼트 설정
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

        // 다이얼로그 세그먼트 프로세스 코루틴 시작 및 저장
        _currentSegmentCoroutine = StartCoroutine(SegmentCoroutine());
    }
    /// <summary> 다이얼로그 세그먼트를 한 글자씩 출력하는 코루틴 </summary>
    private IEnumerator SegmentCoroutine()
    {
        // 한 프레임 쉬고 시작
        yield return null;

        // 세그먼트의 대사 크기만큼 StringBuilder 생성
        StringBuilder stringBuilder = new StringBuilder(_currentSegment.Text.Length);

        int textIndex = 0;
        int playSoundCharacterIndex = 0;

        // 세그먼트의 대사를 한 글자씩 순회
        while (true)
        {
            // 이는 태그를 파싱하기 위함
            if (_currentSegment.Text[textIndex] == '<')
            {
                int from = textIndex;
                int to = _currentSegment.Text.IndexOf('>', from);
                string textTag = _currentSegment.Text.Substring(from, to - from + 1);       // from을 포함한 length만큼 substring

                stringBuilder.Append(textTag);

                textIndex = to;
            }
            // 텍스트 천천히 출력
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
            // 일반 텍스트를 stringBuilder에 추가
            else
            {
                stringBuilder.Append(_currentSegment.Text[textIndex]);
                _dialogue.text = stringBuilder.ToString();      // 현재까지의 stringBuilder를 텍스트로 출력

                // 글자 출력 사운드 재생

                if(playSoundCharacterIndex % CountOfPlaySoundCharacter == 0)
                {
                    SoundManager.Instance.PlayCommonSFX("SE_UI_Script" + UnityEngine.Random.Range(1, 6).ToString());
                    playSoundCharacterIndex = 0;
                }

                playSoundCharacterIndex++;
            }

            // 다음 글자로 이동
            textIndex++;

            // 세그먼트의 대사를 모두 출력했을 경우 종료, 또는 강제종료
            if (textIndex == _currentSegment.Text.Length)
                break;

            yield return new WaitForSeconds(_currentSegment.CharShowInterval);
        }

        // 세그먼트 마무리 단계
        CleanUpOnSegmentOver();
    }
    /// <summary> 다이얼로그 세그먼트 서서히 사라짐 </summary>
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
    /// <summary> 다이얼로그 세그먼트에서 [Time]을 제외 </summary>
    private string ExtractTextWithoutTime(string originText)
    {
        StringBuilder result = new StringBuilder(originText.Length);

        for (int i = 0; i < originText.Length; i++)
        {
            if (originText[i] == '[')
            {
                var from = i;
                var to = originText.IndexOf(']', from);

                // 못찾은 경우 에러를 발생하도록 하고, 대괄호를 문자로 추가한다
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
    /// <summary> 다이얼로그 세그먼트 빠르게 넘어가기 </summary>
    public void FastForward()
    {
        StopCoroutine(_currentSegmentCoroutine);
        CleanUpOnSegmentOver();
    }
    /// <summary> 다이얼로그 세그먼트 종료 처리 </summary>
    public void CleanUpOnSegmentOver()
    {
        IsCurrentSegmentOver = true;
        _dialogue.text = _exceptTimeSegmentText;
        _indicator.gameObject.SetActive(true);
    }

    public void SkipDialogue()
    {
        if(DialogueController.Instance)
            DialogueController.Instance.SkipDialogue();
    }
}
