/// <summary>
/// 실행 중인 퀘스트를 관리하고
/// 퀘스트 데이터를 뷰에 전달하는 클래스
/// </summary>
public class QuestController : HappyTools.SingletonBehaviourFixed<QuestController>
{
    private QuestData _currentQuest;        // 현재 실행 중인 퀘스트
    private QuestView _view;
    public QuestView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<QuestView>(true);
            return _view;
        }
    }

    public QuestData CurrentQuest => _currentQuest;

    public void AcceptQuest(QuestData questData)
    {
        // 현재 실행 중인 퀘스트로 등록
        _currentQuest = questData;

        // 퀘스트 활성화
        _currentQuest.IsActive = true;

        _currentQuest.IsAcceptedBefore = true;

        // 퀘스트 데이터를 뷰에 전달
        View.UpdatePanel(_currentQuest);

        // 퀘스트 패널 열기
        View.OpenPanel();
    }
    public void RejectQuest(QuestData questData)
    {
        questData.IsAcceptedBefore = false;
    }
    public void CompleteQuest()
    {
        // 퀘스트 패널 닫기
        View.ClosePanel();

        // 퀘스트 완료 이벤트 발생
        _currentQuest.CompleteQuestProcess();

        // 현재 실행 중인 퀘스트 해제
        _currentQuest = null;
    }
    public void UpdateQuest()
    {
        // 퀘스트 데이터를 뷰에 전달
        View.UpdatePanel(_currentQuest);
    }
}
