using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public Quest CurrentQuest { get; private set; }

    public void SetQuest(Quest quest)
    {
        CurrentQuest = quest;
    }
    public void Accept()
    {
        // Quest를 등록한다
        QuestController.Instance.RequestQuest(CurrentQuest);

        this.gameObject.SetActive(false);
    }
    public void Reject()
    {
        this.gameObject.SetActive(false);
    }
}
