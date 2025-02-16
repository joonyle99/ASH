using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다이얼로그를 시작하고 종료하는 역할을 한다
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _dialogueSegmentFadeTime;

    public bool IsDialoguePanel => View.IsDialoguePanelActive;          // 대화 패널이 열려있는지 여부
    public bool IsDialogueActive { get; set; } = false;                 // 대화가 진행 중인지 여부

    private DialogueView _view;                                         // 다이얼로그 뷰 UI
    public DialogueView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>(true);
            return _view;
        }
    }

    private Coroutine _currentDialogueCoroutine;
    [SerializeField]
    private DialogueData _currentDialogueData;

    private bool _isSkipSequence = false;

    public void StartDialogue(DialogueData data, bool isContinueDialogue = false)
    {
        if (IsDialogueActive)
        {
            Debug.Log("대화가 이미 진행중입니다");
            return;
        }

        if (_currentDialogueCoroutine != null)
        {
            Debug.LogError($"_currentDialogueCoroutine is not 'null'");
            return;
        }

        _currentDialogueData = data;
        _currentDialogueCoroutine = StartCoroutine(DialogueCoroutine(data, isContinueDialogue, !data.PlayAtFirst));
    }

    private IEnumerator DialogueCoroutine(DialogueData data, bool isContinueDialogue = false, bool canSkip = false)
    {
        // 1. 다이얼로그 시퀀스를 생성한다
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // 2. 입력 설정이 있을 경우 변경
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        // 3. 다이얼로그 뷰 UI를 열어준다
        View.OpenPanel(canSkip);

        IsDialogueActive = true;

        // 4. 다이얼로그 시퀀스 시작
        while (dialogueSequence.IsOver == false)
        {
            #region Dialogue

            // 다이얼로그 뷰 UI에 현재 세그먼트를 표시
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // 진행중인 다이얼로그 세그먼트가 끝날 때까지 루프를 돌며 대기
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                // (*변경)스킵버튼을 누르거나 키다운시 스킵
                // 어떤 키라도 눌러졌다면 스킵
                if (canSkip && (_isSkipSequence || Input.anyKeyDown))
                {
                    View.FastForward();
                    _isSkipSequence = false;

                    yield return null;
                }

                // CHEAT: F3 키를 누르면 현재 Segment를 빠르게 넘긴다
                if (Input.GetKeyDown(KeyCode.F3) && GameSceneManager.Instance.CheatMode == true)
                {
                    View.FastForward();
                }
            }

            yield return new WaitUntil(() => Input.anyKeyDown);

            SoundManager.Instance.PlayCommonSFX("SE_UI_Select");

            #endregion

            #region Quest

            // 마지막 다이얼로그 세그먼트인 경우 퀘스트가 등록되어 있는지 확인
            if (dialogueSequence.IsLastSegment)
            {
                // 다이얼로그에 퀘스트가 등록되어 있는 경우
                if (data.Quest != null)
                {
                    if (data.Quest.IsAutoFirst == true)
                    {
                        data.Quest.IsAutoFirst = false;
                        yield return new WaitForSeconds(1f);
                        QuestController.Instance.AcceptQuest(data.Quest);
                    }
                    else
                    {
                        // 수락 / 버튼 리스너 컨테이너 생성
                        List<ResponseContainer> container = new List<ResponseContainer>();
                        container.Add(new ResponseContainer(ResponseButtonType.Accept, () => data.Quest.OnAccept?.Invoke()));
                        container.Add(new ResponseContainer(ResponseButtonType.Reject, () => data.Quest.OnReject?.Invoke()));

                        // 퀘스트 응답 패널을 연다 (+ 리스너 컨테이너 등록)
                        View.OpenResponsePanel(container);

                        // Handler: 이벤트가 발생했을 때 호출되는 함수를 지칭한다 (옵저버 패턴)
                        var isClicked = false;
                        void ResponseHandler()
                        {
                            isClicked = true;
                            View.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
                            View.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
                        }
                        View.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
                        View.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
                        View.ResponsePanel.Accept.onClick.AddListener(ResponseHandler);
                        View.ResponsePanel.Reject.onClick.AddListener(ResponseHandler);

                        // 해당 퀘스트가 수락 / 거절되기 전까지 대기
                        yield return new WaitUntil(() => isClicked);
                    }

                    // 퀘스트 응답 종료 사운드 재생
                    SoundManager.Instance.PlayCommonSFX("SE_UI_Select");
                }
            }

            #endregion

            // 다이얼로그 세그먼트가 끝난 후 대기 시간만큼 대기
            yield return StartCoroutine(View.ClearTextCoroutine(_dialogueSegmentFadeTime));

            // 다음 다이얼로그 세그먼트로 이동
            dialogueSequence.MoveNext();
        }

        // 5. 다이얼로그 뷰 UI를 닫아준다
        View.ClosePanel();

        if (!isContinueDialogue)
            IsDialogueActive = false;

        // 6. 다이얼로그 시퀀스가 끝났기 때문에 입력 설정을 기본값으로 변경
        if (data.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();

        SetCurrentDialogueData(false);
        _currentDialogueCoroutine = null;
        _currentDialogueData = null;
    }

    public void ShutdownDialogue()
    {
        if (_currentDialogueCoroutine == null)
        {
            // Debug.Log("대화가 진행 중이 아닙니다");
            return;
        }

        if (_currentDialogueData == null)
        {
            Debug.LogError("대화가 진행 중이지만 대화 데이터가 존재하지 않습니다");
            return;
        }

        SoundManager.Instance.PlayCommonSFX("SE_UI_Select");

        if (_currentDialogueData.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();

        StopCoroutine(_currentDialogueCoroutine);
        _currentDialogueCoroutine = null;
        _currentDialogueData = null;

        View.StopAllCoroutines();
        View.CleanUpOnSegmentOver();
        View.ClosePanel();

        IsDialogueActive = false;
    }

    public void SkipDialogue()
    {
        if (!View.IsCurrentSegmentOver)
        {
            _isSkipSequence = true;
        }
    }

    private void SetCurrentDialogueData(bool playAtFirst)
    {
        if (_currentDialogueData)
        {
            _currentDialogueData.PlayAtFirst = playAtFirst;
        }
    }
}
