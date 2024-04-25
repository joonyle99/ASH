using UnityEngine.UI;
using TMPro;

public class QuestController : joonyleTools.SingletonBehavior<QuestController>
{
    private Quest _currentQuest;
    private bool _isQuestActive;

    public QuestView View;

    public void RequestQuest(Quest quest)
    {
        _currentQuest = quest;
        _isQuestActive = true;

        // UI에 퀘스트 정보 표시한다
        View.Open(quest);
    }

    public void CompleteQuest()
    {
        _currentQuest.Reward.Invoke();

        _currentQuest = null;
        _isQuestActive = false;
    }
}
