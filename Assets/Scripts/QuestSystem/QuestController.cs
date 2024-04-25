/// <summary>
/// 실행 중인 퀘스트를 관리하고
/// 퀘스트 데이터를 뷰에 전달하는 클래스
/// </summary>
public class QuestController : joonyleTools.SingletonBehavior<QuestController>
{
    private QuestData _currentQuest;
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

    public void AcceptQuest(QuestData questData)
    {
        // 현재 실행 중인 퀘스트로 등록
        _currentQuest = questData;

        // 퀘스트 활성화
        _currentQuest.IsActive = true;

        // 퀘스트 데이터를 뷰에 전달
        View.DrawDataOnQuestPanel(_currentQuest);

        // 퀘스트 패널 열기
        View.OpenQuestPanel();
    }

    public void CompleteQuest()
    {
        // 퀘스트 보상 지급
        _currentQuest.Reward?.Invoke();

        // 퀘스트 비활성화
        _currentQuest.IsActive = false;

        // 퀘스트 패널 닫기
        View.ClosePanel();

        // 현재 실행 중인 퀘스트 해제
        _currentQuest = null;
    }
}
