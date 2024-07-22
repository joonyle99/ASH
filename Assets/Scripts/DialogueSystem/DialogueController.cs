using System.Collections;
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

    private bool _isShutdowned = false;
    public bool IsShutdowned => _isShutdowned;

    public void StartDialogue(DialogueData data, bool isFromCutscene = false)
    {
        // 대화가 이미 진행 중이라면 종료
        if (IsDialoguePanel)
        {
            Debug.Log("대화가 이미 진행중입니다");
            return;
        }

        StartCoroutine(DialogueCoroutine(data));
    }
    public IEnumerator StartDialogueCoroutine(DialogueData data, bool isContinueDialogue = false)
    {
        // 대화가 이미 진행 중이라면 종료
        if (IsDialoguePanel && IsDialogueActive)
        {
            Debug.Log("대화가 이미 진행중입니다");
            yield break;
        }

        yield return StartCoroutine(DialogueCoroutine(data, isContinueDialogue));
    }
    private IEnumerator DialogueCoroutine(DialogueData data, bool isContinueDialogue = false)
    {
        //0. 기본값 초기화
        _isShutdowned = false;

        // 1. 다이얼로그 시퀀스를 생성한다
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // 2. 입력 설정이 있을 경우 변경
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        // 3. 다이얼로그 뷰 UI를 열어준다
        View.OpenPanel();

        IsDialogueActive = true;

        // 4. 다이얼로그 시퀀스 시작
        while (!dialogueSequence.IsOver && !_isShutdowned)
        {
            #region Dialogue

            // 다이얼로그 뷰 UI에 현재 세그먼트를 표시
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // 진행중인 다이얼로그 세그먼트가 끝날 때까지 루프를 돌며 대기
            while (!View.IsCurrentSegmentOver && !_isShutdowned)
            {
                yield return null;

                if (Input.GetKeyDown(KeyCode.F3))
                    View.FastForward();
            }

            // 다음 Update 프레임까지 대기하며 상호작용 키의 중복 입력을 방지
            yield return null;

            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown || _isShutdowned);

            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

            #endregion

            #region Quest

            // 마지막 다이얼로그 세그먼트인 경우 퀘스트 다이얼로그임을 확인한다
            if (dialogueSequence.IsLastSegment)
            {
                // 다이얼로그에 퀘스트가 등록되어 있는 경우
                if (data.QuestData)
                {
                    // 퀘스트를 처음 받은 경우, 자동 수락
                    if (data.QuestData.IsFirst)
                    {
                        data.QuestData.IsFirst = false;

                        QuestController.Instance.AcceptQuest(data.QuestData);
                    }
                    else
                    {
                        // 퀘스트 응답 패널을 연다
                        View.OpenResponsePanel();

                        // 퀘스트 응답 패널에 퀘스트 데이터를 전달
                        View.SendQuestDataToResponsePanel(data.QuestData, out var response);

                        // Handler: 이벤트가 발생했을 때 호출되는 함수를 지칭한다 (옵저버 패턴)
                        var isClicked = false;
                        void ResponseHandler()
                        {
                            isClicked = true;
                            response.OnClicked -= ResponseHandler;
                        }
                        response.OnClicked -= ResponseHandler;
                        response.OnClicked += ResponseHandler;

                        // 해당 퀘스트가 수락 / 거절되기 전까지 대기
                        yield return new WaitUntil(() => isClicked);

                        // 퀘스트 응답 종료 사운드 재생
                        SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
                    }
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
    }

    public void ShutdownDialogue()
    {
        _isShutdowned = true;
    }
}
