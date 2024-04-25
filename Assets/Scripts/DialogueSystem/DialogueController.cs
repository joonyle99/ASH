using System.Collections;
using UnityEngine;

/// <summary>
/// ���̾�α׸� �����ϰ� �����ϴ� ������ �Ѵ�
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _waitTimeAfterScriptEnd;             // ��ȭ�� ���� �� ��� �ð�

    public bool IsDialogueActive => View.IsDialoguePanelActive;         // ���̾�α� �䰡 Ȱ��ȭ ������ ����

    private DialogueView _view;                                         // ���̾�α� �� UI
    private DialogueView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>(true);
            return _view;
        }
    }

    /// <summary>
    /// ���̾�α� ���� �Լ�
    /// </summary>
    /// <param name="data"></param>
    /// <param name="isFromCutscene"></param>
    public void StartDialogue(DialogueData data, bool isFromCutscene = false)
    {
        // ��ȭ�� �̹� ���� ���̶�� ����
        if (IsDialogueActive) return;

        StartCoroutine(DialogueCoroutine(data));
    }
    /// <summary>
    /// ���̾�α� ���� �ڷ�ƾ
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator DialogueCoroutine(DialogueData data)
    {
        // ���̾�α� �����͸� ���� ���̾�α� ������ ����
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // �Է� ������ ������ ����
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        // ���̾�α� �� UI�� �����ش�
        View.OpenPanel();

        // ���̾�α� ������ ����
        while (!dialogueSequence.IsOver)
        {
            // ���̾�α� �� UI�� ���� ���׸�Ʈ�� ǥ��
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // �������� ���̾�α� ���׸�Ʈ�� ���� ������ ������ ���� ���
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                if (InputManager.Instance.State.InteractionKey.KeyDown)
                    View.FastForward();
            }

            // ���� Update �����ӱ��� ����ϸ� ��ȣ�ۿ� Ű�� �ߺ� �Է��� ����
            yield return null;

            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

            // ������ ���̾�α� ���׸�Ʈ�� ��� ����Ʈ ���̾�α����� Ȯ���Ѵ�
            if (dialogueSequence.IsLastSegment)
            {
                if (data.QuestData.IsValidQuestData())
                {
                    // ����Ʈ ���� �г��� ����
                    View.OpenResponsePanel();

                    // ����Ʈ ���� �гο� ����Ʈ �����͸� ����
                    View.SendQuestDataToResponsePanel(data.QuestData, out var response);

                    // Handler: �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �Լ��� ��Ī�Ѵ� (������ ����)
                    var isClicked = false;
                    void EventHandler()
                    {
                        isClicked = true;
                        response.OnClicked -= EventHandler;
                    }
                    response.OnClicked -= EventHandler;
                    response.OnClicked += EventHandler;

                    // �ش� ����Ʈ�� ���� / �����Ǳ� ������ ���
                    yield return new WaitUntil(() => isClicked);

                    // ����Ʈ ���� ���� ���� ���
                    SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
                }
            }

            // ���̾�α� ���׸�Ʈ�� ���� �� ��� �ð���ŭ ���
            yield return StartCoroutine(View.ClearTextCoroutine(_waitTimeAfterScriptEnd));

            // ���� ���̾�α� ���׸�Ʈ�� �̵�
            dialogueSequence.MoveNext();
        }

        // ���̾�α� �� UI�� �ݾ��ش�
        View.ClosePanel();

        // ���̾�α� �������� ������ ������ �Է� ������ �⺻������ ����
        if (data.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();
    }

}
