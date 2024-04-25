/// <summary>
/// ���� ���� ����Ʈ�� �����ϰ�
/// ����Ʈ �����͸� �信 �����ϴ� Ŭ����
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
        // ���� ���� ���� ����Ʈ�� ���
        _currentQuest = questData;

        // ����Ʈ Ȱ��ȭ
        _currentQuest.IsActive = true;

        // ����Ʈ �����͸� �信 ����
        View.DrawDataOnQuestPanel(_currentQuest);

        // ����Ʈ �г� ����
        View.OpenQuestPanel();
    }

    public void CompleteQuest()
    {
        // ����Ʈ ���� ����
        _currentQuest.Reward?.Invoke();

        // ����Ʈ ��Ȱ��ȭ
        _currentQuest.IsActive = false;

        // ����Ʈ �г� �ݱ�
        View.ClosePanel();

        // ���� ���� ���� ����Ʈ ����
        _currentQuest = null;
    }
}
