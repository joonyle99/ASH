using UnityEngine;

/// <summary>
/// 전달받은 퀘스트에 대한 응답을 처리하는 클래스
/// </summary>
public class QuestResponse : MonoBehaviour
{
    private QuestData _questData;

    public delegate void OnQuestResponseClicked();              // 메서드 참조를 변수에 할당할 수 있게 해주는 'delegate'를 선언
    public event OnQuestResponseClicked OnClicked;              // 대리자 변수 선언 'event'를 사용하면 외부에서 대리자 변수에 직접 접근할 수 없다
                                                                // 'Action'를 사용할 수도 있는데, 이는 반환값이 없는 대리자를 선언할 때 사용한다 (선언이 매우 간단하다)

    public void ReceiveQuestData(QuestData questData)
    {
        _questData = questData;
    }

    public void AcceptQuest()
    {
        // 퀘스트를 수락한다
        QuestController.Instance.AcceptQuest(_questData);
        OnClicked?.Invoke();
    }
    public void RejectQuest()
    {
        // 퀘스트를 거절한다
        OnClicked?.Invoke();
    }
}
