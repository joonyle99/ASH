/// <summary>
/// ���� ���� ����Ʈ�� �����ϰ�
/// ����Ʈ �����͸� �信 �����ϴ� Ŭ����
/// </summary>
public class QuestController : HappyTools.SingletonBehaviourFixed<QuestController>
{
    private QuestData _currentQuest;        // ���� ���� ���� ����Ʈ
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
        // ���� ���� ���� ����Ʈ�� ���
        _currentQuest = questData;

        // ����Ʈ Ȱ��ȭ
        _currentQuest.IsActive = true;

        _currentQuest.IsAcceptedBefore = true;

        // ����Ʈ �����͸� �信 ����
        View.UpdatePanel(_currentQuest);

        // ����Ʈ �г� ����
        View.OpenPanel();
    }
    public void RejectQuest(QuestData questData)
    {
        questData.IsAcceptedBefore = false;
    }
    public void CompleteQuest()
    {
        // ����Ʈ �г� �ݱ�
        View.ClosePanel();

        // ����Ʈ �Ϸ� �̺�Ʈ �߻�
        _currentQuest.CompleteQuestProcess();

        // ���� ���� ���� ����Ʈ ����
        _currentQuest = null;
    }
    public void UpdateQuest()
    {
        // ����Ʈ �����͸� �信 ����
        View.UpdatePanel(_currentQuest);
    }
}
