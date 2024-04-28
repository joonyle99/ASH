using UnityEngine;

/// <summary>
/// ���޹��� ����Ʈ�� ���� ������ ó���ϴ� Ŭ����
/// </summary>
public class QuestResponse : MonoBehaviour
{
    private QuestData _questData;

    public delegate void OnQuestResponseClicked();              // �޼��� ������ ������ �Ҵ��� �� �ְ� ���ִ� 'delegate'�� ����
    public event OnQuestResponseClicked OnClicked;              // �븮�� ���� ���� 'event'�� ����ϸ� �ܺο��� �븮�� ������ ���� ������ �� ����
                                                                // 'Action'�� ����� ���� �ִµ�, �̴� ��ȯ���� ���� �븮�ڸ� ������ �� ����Ѵ� (������ �ſ� �����ϴ�)

    public void ReceiveQuestData(QuestData questData)
    {
        _questData = questData;
    }

    public void AcceptQuest()
    {
        // ����Ʈ�� �����Ѵ�
        QuestController.Instance.AcceptQuest(_questData);
        OnClicked?.Invoke();
    }
    public void RejectQuest()
    {
        // ����Ʈ�� �����Ѵ�
        OnClicked?.Invoke();
    }
}
