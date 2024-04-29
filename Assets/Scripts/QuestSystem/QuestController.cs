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

    public QuestData CurrentQuest => _currentQuest;
    public bool IsCurrentQuestActive => _currentQuest != null;

    // TODO: ����Ʈ �̺�Ʈ�� ����ϱ� ���� �븮�� ����
    public delegate void QuestEvent();
    public event QuestEvent questEvent;

    protected override void Awake()
    {
        // TODO: �̹� Bootstrap�� �ڽ����� �����Ѵ�
        // base.Awake();

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

    public void UpdateQuest()
    {
        // ����Ʈ �����͸� �信 ����
        View.DrawDataOnQuestPanel(_currentQuest);
    }

    public void CompleteQuest()
    {
        // ����Ʈ ���� ����
        _currentQuest.MonsterQuest.reward?.Invoke();

        // ����Ʈ ��Ȱ��ȭ
        _currentQuest.IsActive = false;

        // ����Ʈ ������ �ʱ�ȭ
        _currentQuest.InitializeQuestData();

        // ����Ʈ �г� �ݱ�
        View.ClosePanel();

        // ���� ���� ���� ����Ʈ ����
        _currentQuest = null;
    }
}
