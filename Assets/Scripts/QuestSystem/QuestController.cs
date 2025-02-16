using UnityEngine;

/// <summary>
/// 실행 중인 퀘스트를 관리하고
/// 퀘스트 데이터를 뷰에 전달하는 클래스
/// </summary>
public class QuestController : HappyTools.SingletonBehaviourFixed<QuestController>
{
    [SerializeField] private Quest _globalQuest;
    public Quest GlobalQuest => _globalQuest;

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

    public void AcceptQuest()
    {
        _globalQuest.IsActive = true;
        _globalQuest.IsAcceptedBefore = true;

        View.UpdatePanel(_globalQuest);
        View.OpenPanel();
    }
    public void RejectQuest()
    {
        _globalQuest.IsAcceptedBefore = false;
    }
    public void AcceptQuest(Quest quest)
    {
        quest.IsActive = true;
        quest.IsAcceptedBefore = true;

        View.UpdatePanel(quest);
        View.OpenPanel();

        _globalQuest = quest;
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
        _globalQuest.CompleteProcess();
    }
    public void UpdateQuest()
    {
        // 퀘스트 데이터를 뷰에 전달
        View.UpdatePanel(_globalQuest);
    }
}
