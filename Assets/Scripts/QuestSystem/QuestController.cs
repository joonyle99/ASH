using UnityEngine;

/// <summary>
/// 실행 중인 퀘스트를 관리하고
/// 퀘스트 데이터를 뷰에 전달하는 클래스
/// </summary>
public class QuestController : HappyTools.SingletonBehaviourFixed<QuestController>
{
    private Quest _currentQuest;
    public Quest CurrentQuest => _currentQuest;

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

    public void AcceptQuest(Quest quest)
    {
        // 싱글톤 객체인 QuestController에 새로운 퀘스트 인스턴스를 생성
        // 얕은 복사이지만 퀘스트는 값만 가지고 있기 때문에 상관 없음 (QuestData는 전역 참조)
        // _currentQuest = new Quest(quest);

        // 참조 할당 씬의 객체와 동기화된다는 장점이 있지만,
        // 씬을 이동하면 해당 객체는 파괴되기 때문에 참조가 끊김.

        // DontDestroyOnLoad(quest.gameObject);

        quest.IsActive = true;
        quest.IsAcceptedBefore = true;

        View.UpdatePanel(quest);
        View.OpenPanel();

        _currentQuest = quest;
    }
    public void RejectQuest(Quest quest)
    {
        quest.IsAcceptedBefore = false;
    }
    public void CompleteQuest()
    {
        // 퀘스트 패널 닫기
        View.ClosePanel();

        // 퀘스트 완료 이벤트 발생
        _currentQuest.CompleteProcess();

        // 현재 실행 중인 퀘스트 해제
        _currentQuest = null;
    }
    public void UpdateQuest()
    {
        // 퀘스트 데이터를 뷰에 전달
        View.UpdatePanel(_currentQuest);
    }
}
