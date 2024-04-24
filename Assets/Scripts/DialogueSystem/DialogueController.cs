using System.Collections;
using UnityEngine;

/// <summary>
/// 다이얼로그를 관리하는 컨트롤러
/// 다이얼로그를 시작하고 종료하는 역할을 한다
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _waitTimeAfterScriptEnd;     // 대화가 끝난 후 대기 시간

    public bool IsDialogueActive => View.IsPanelActive;         // 다이얼로그 뷰가 활성화 중인지 여부

    private DialogueView _view;                                 // 다이얼로그 뷰 UI
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
    /// <param name="data">다이얼로그에 대한 모든 정보를 담는 데이터</param>
    /// <param name="isFromCutscene">컷씬으로부터 온 다이얼로그인지 확인하기 위한 부울 값</param>
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
        // 다이얼로그 데이터를 통해 객체 생성
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // 다이얼로그 데이터 입력 설정이 있으면 변경
        if (data.InputSetter != null)
        {
            InputManager.Instance.ChangeInputSetter(data.InputSetter);
        }

        // 다이얼로그 뷰 UI를 열어준다
        View.OpenPanel();

        // 다이얼로그 시퀀스 시작
        while (!dialogueSequence.IsOver)
        {
            // 다이얼로그 뷰 UI에 현재 세그먼트를 표시
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // 다이얼로그 세그먼트가 끝날 때까지 대기
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                // 텍스트를 빠르게 표시
                if (InputManager.Instance.State.InteractionKey.KeyDown)
                    View.FastForward();
            }

            // 다음 Update까지 대기
            yield return null;

            // 상호작용 키를 이용해 다이얼로그 세그먼트를 종료하기 전까지 대기한다
            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            // 다이얼로그 세그먼트 종료 사운드 재생
            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

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
