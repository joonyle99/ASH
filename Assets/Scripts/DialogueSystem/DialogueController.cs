using System.Collections;
using UnityEngine;

/// <summary>
/// ���̾�α׸� �����ϰ� �����ϴ� ������ �Ѵ�
/// </summary>
public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{
    [Header("Dialogue Controller")]
    [Space]

    [SerializeField] private float _dialogueSegmentFadeTime;

    public bool IsDialoguePanel => View.IsDialoguePanelActive;          // ��ȭ �г��� �����ִ��� ����
    public bool IsDialogueActive { get; set; } = false;                 // ��ȭ�� ���� ������ ����

    private DialogueView _view;                                         // ���̾�α� �� UI
    public DialogueView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>(true);
            return _view;
        }
    }

    private Coroutine _currentDialogueCoroutine;

    public void StartDialogue(DialogueData data, bool isFromCutscene = false)
    {
        // ��ȭâ�� ���� ������ �Ǵ�
        if (IsDialoguePanel)
        {
            Debug.Log("��ȭ�� �̹� �������Դϴ�");
            return;
        }

        if (_currentDialogueCoroutine != null)
        {
            Debug.LogError($"_currentDialogueCoroutine is not 'null'");
            return;
        }

        _currentDialogueCoroutine = StartCoroutine(DialogueCoroutine(data));
    }
    private IEnumerator DialogueCoroutine(DialogueData data, bool isContinueDialogue = false)
    {
        // 1. ���̾�α� �������� �����Ѵ�
        DialogueSequence dialogueSequence = new DialogueSequence(data);

        // 2. �Է� ������ ���� ��� ����
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        // 3. ���̾�α� �� UI�� �����ش�
        View.OpenPanel();

        IsDialogueActive = true;

        // 4. ���̾�α� ������ ����
        while (!dialogueSequence.IsOver)
        {
            #region Dialogue

            // ���̾�α� �� UI�� ���� ���׸�Ʈ�� ǥ��
            View.StartNextSegment(dialogueSequence.CurrentSegment);

            // �������� ���̾�α� ���׸�Ʈ�� ���� ������ ������ ���� ���
            while (!View.IsCurrentSegmentOver)
            {
                yield return null;

                if (Input.GetKeyDown(KeyCode.F3))
                    View.FastForward();
            }

            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

            #endregion

            #region Quest

            // ������ ���̾�α� ���׸�Ʈ�� ��� ����Ʈ ���̾�α����� Ȯ���Ѵ�
            if (dialogueSequence.IsLastSegment)
            {
                // ���̾�α׿� ����Ʈ�� ��ϵǾ� �ִ� ���
                if (data.QuestData)
                {
                    // ����Ʈ�� ó�� ���� ���, �ڵ� ����
                    if (data.QuestData.IsFirst)
                    {
                        data.QuestData.IsFirst = false;

                        QuestController.Instance.AcceptQuest(data.QuestData);
                    }
                    else
                    {
                        // ����Ʈ ���� �г��� ����
                        View.OpenResponsePanel();

                        // ����Ʈ ���� �гο� ����Ʈ �����͸� ����
                        View.SendQuestDataToResponsePanel(data.QuestData, out var response);

                        // Handler: �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �Լ��� ��Ī�Ѵ� (������ ����)
                        var isClicked = false;
                        void ResponseHandler()
                        {
                            isClicked = true;
                            response.OnClicked -= ResponseHandler;
                        }
                        response.OnClicked -= ResponseHandler;
                        response.OnClicked += ResponseHandler;

                        // �ش� ����Ʈ�� ���� / �����Ǳ� ������ ���
                        yield return new WaitUntil(() => isClicked);

                        // ����Ʈ ���� ���� ���� ���
                        SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
                    }
                }
            }

            #endregion

            // ���̾�α� ���׸�Ʈ�� ���� �� ��� �ð���ŭ ���
            yield return StartCoroutine(View.ClearTextCoroutine(_dialogueSegmentFadeTime));

            // ���� ���̾�α� ���׸�Ʈ�� �̵�
            dialogueSequence.MoveNext();
        }

        // 5. ���̾�α� �� UI�� �ݾ��ش�
        View.ClosePanel();

        if (!isContinueDialogue)
            IsDialogueActive = false;

        // 6. ���̾�α� �������� ������ ������ �Է� ������ �⺻������ ����
        if (data.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();

        _currentDialogueCoroutine = null;
    }

    // TEMP
    public IEnumerator StartDialogueCoroutine(DialogueData data, bool isContinueDialogue = false)
    {
        // ��ȭ�� �̹� ���� ���̶�� ����
        if (IsDialoguePanel && IsDialogueActive)
        {
            Debug.Log("��ȭ�� �̹� �������Դϴ�");
            yield break;
        }

        yield return StartCoroutine(DialogueCoroutine(data, isContinueDialogue));
    }

    public void ShutdownDialogue()
    {
        if (_currentDialogueCoroutine == null)
        {
            Debug.Log("��ȭ�� ���� ���� �ƴմϴ�");
            return;
        }

        SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");

        StopCoroutine(_currentDialogueCoroutine);
        _currentDialogueCoroutine = null;

        View.StopAllCoroutines();

        View.CleanUpOnSegmentOver();
        View.ClosePanel();
        IsDialogueActive = false;

        InputManager.Instance.ChangeToDefaultSetter();
    }
}
