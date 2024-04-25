using System.Collections;
using UnityEngine;

/// <summary>
/// 다이얼로그를 시작하고 종료하는 역할을 한다
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _waitTimeAfterScriptEnd;             // 대화가 끝난 후 대기 시간

    public bool IsDialogueActive => View.IsDialoguePanelActive;         // 다이얼로그 뷰가 활성화 중인지 여부

    private DialogueView _view;                                         // 다이얼로그 뷰 UI
    private DialogueView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>(true);
            return _view;
        }
    }

    /// <summary>
    /// 다이얼로그 시작 함수
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isFromCutscene"></param>
    public void StartDialogue(DialogueData data, bool isFromCutscene = false)
    {
        // 대화가 이미 진행 중이라면 종료
        if (IsDialogueActive) return;

        StartCoroutine(DialogueCoroutine(data));
    }
    /// <summary>
    /// 다이얼로그 시작 코루틴
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator DialogueCoroutine(DialogueData data)
    {
        // 다이얼로그 데이터를 통해 다이얼로그 시퀀스 생성
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // 입력 설정이 있으면 변경
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        // 다이얼로그 뷰 UI를 열어준다
        View.OpenPanel();

        // 다이얼로그 시퀀스 시작
        while (!dialogueSequence.IsOver)
        {
            // 다이얼로그 뷰 UI에 현재 세그먼트를 표시
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // 진행중인 다이얼로그 세그먼트가 끝날 때까지 루프를 돌며 대기
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                if (InputManager.Instance.State.InteractionKey.KeyDown)
                    View.FastForward();
            }

            // 다음 Update 프레임까지 대기하며 상호작용 키의 중복 입력을 방지
            yield return null;

            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

            // 마지막 다이얼로그 세그먼트인 경우 퀘스트 다이얼로그임을 확인한다
            if (dialogueSequence.IsLastSegment)
            {
                if (data.QuestData.IsValidQuestData())
                {
                    // 퀘스트 응답 패널을 연다
                    View.OpenResponsePanel();

                    // 퀘스트 응답 패널에 퀘스트 데이터를 전달
                    View.SendQuestDataToResponsePanel(data.QuestData, out var response);

                    // Handler: 이벤트가 발생했을 때 호출되는 함수를 지칭한다 (옵저버 패턴)
                    var isClicked = false;
                    void EventHandler()
                    {
                        isClicked = true;
                        response.OnClicked -= EventHandler;
                    }
                    response.OnClicked -= EventHandler;
                    response.OnClicked += EventHandler;

                    // 해당 퀘스트가 수락 / 거절되기 전까지 대기
                    yield return new WaitUntil(() => isClicked);

                    // 퀘스트 응답 종료 사운드 재생
                    SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
                }
            }

            // 다이얼로그 세그먼트가 끝난 후 대기 시간만큼 대기
            yield return StartCoroutine(View.ClearTextCoroutine(_waitTimeAfterScriptEnd));

            // 다음 다이얼로그 세그먼트로 이동
            dialogueSequence.MoveNext();
        }

        // 다이얼로그 뷰 UI를 닫아준다
        View.ClosePanel();

        // 다이얼로그 시퀀스가 끝났기 때문에 입력 설정을 기본값으로 변경
        if (data.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();
    }

}
