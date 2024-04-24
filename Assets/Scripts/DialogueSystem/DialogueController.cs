using System.Collections;
using UnityEngine;

/// <summary>
/// ���̾�α׸� �����ϴ� ��Ʈ�ѷ�
/// ���̾�α׸� �����ϰ� �����ϴ� ������ �Ѵ�
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _waitTimeAfterScriptEnd;     // ��ȭ�� ���� �� ��� �ð�

    public bool IsDialogueActive => View.IsPanelActive;         // ���̾�α� �䰡 Ȱ��ȭ ������ ����

    private DialogueView _view;                                 // ���̾�α� �� UI
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
    /// <param name="data">���̾�α׿� ���� ��� ������ ��� ������</param>
    /// <param name="isFromCutscene">�ƾ����κ��� �� ���̾�α����� Ȯ���ϱ� ���� �ο� ��</param>
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
        // ���̾�α� �����͸� ���� ��ü ����
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // ���̾�α� ������ �Է� ������ ������ ����
        if (data.InputSetter != null)
        {
            InputManager.Instance.ChangeInputSetter(data.InputSetter);
        }

        // ���̾�α� �� UI�� �����ش�
        View.OpenPanel();

        // ���̾�α� ������ ����
        while (!dialogueSequence.IsOver)
        {
            // ���̾�α� �� UI�� ���� ���׸�Ʈ�� ǥ��
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // ���̾�α� ���׸�Ʈ�� ���� ������ ���
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                // �ؽ�Ʈ�� ������ ǥ��
                if (InputManager.Instance.State.InteractionKey.KeyDown)
                    View.FastForward();
            }

            // ���� Update���� ���
            yield return null;

            // ��ȣ�ۿ� Ű�� �̿��� ���̾�α� ���׸�Ʈ�� �����ϱ� ������ ����Ѵ�
            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            // ���̾�α� ���׸�Ʈ ���� ���� ���
            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

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
