/// <summary>
/// ���� ���� ����Ʈ�� �����ϰ�
/// ����Ʈ �����͸� �信 �����ϴ� Ŭ����
/// </summary>
public class QuestController : HappyTools.SingletonBehaviourFixed<QuestController>
{
    // SerializeField�� ����ȭ�ϸ� �⺻������ null�� �ƴ� ���·� �����ϱ� ������ ��� �� ��
    private Quest _currentQuest = null;
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
        // �̱��� ��ü�� QuestController�� ���ο� ����Ʈ �ν��Ͻ��� ����
        // ���� ���������� ����Ʈ�� ���� ������ �ֱ� ������ ��� ���� (QuestData�� ���� ����)
        // _currentQuest = new Quest(quest);

        // ���� �Ҵ� ���� ��ü�� ����ȭ�ȴٴ� ������ ������,
        // ���� �̵��ϸ� �ش� ��ü�� �ı��Ǳ� ������ ������ ����.
        _currentQuest = quest;

        // ����Ʈ Ȱ��ȭ
        _currentQuest.IsActive = true;

        _currentQuest.IsAcceptedBefore = true;

        // ����Ʈ �����͸� �信 ����
        View.UpdatePanel(_currentQuest);

        // ����Ʈ �г� ����
        View.OpenPanel();
    }
    public void RejectQuest(Quest quest)
    {
        quest.IsAcceptedBefore = false;
    }
    public void CompleteQuest()
    {
        // ����Ʈ �г� �ݱ�
        View.ClosePanel();

        // ����Ʈ �Ϸ� �̺�Ʈ �߻�
        _currentQuest.CompleteProcess();

        // ���� ���� ���� ����Ʈ ����
        _currentQuest = null;
    }
    public void UpdateQuest()
    {
        // ����Ʈ �����͸� �信 ����
        View.UpdatePanel(_currentQuest);
    }
}
