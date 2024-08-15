using System.Collections;
using System.Collections.Generic;
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
    private DialogueData _currentDialogueData;

    public void StartDialogue(DialogueData data, bool isContinueDialogue = false)
    {
        if (IsDialogueActive)
        {
            Debug.Log("��ȭ�� �̹� �������Դϴ�");
            return;
        }

        if (_currentDialogueCoroutine != null)
        {
            Debug.LogError($"_currentDialogueCoroutine is not 'null'");
            return;
        }

        _currentDialogueData = data;
        _currentDialogueCoroutine = StartCoroutine(DialogueCoroutine(data, isContinueDialogue));
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

                // CHEAT: F3 Ű�� ������ ��ȭ�� ������ ����
                if (Input.GetKeyDown(KeyCode.F3))
                    View.FastForward();
            }

            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);

            SoundManager.Instance.PlayCommonSFX("SE_UI_Select");

            #endregion

            #region Quest

            // ������ ���̾�α� ���׸�Ʈ�� ��� ����Ʈ�� ��ϵǾ� �ִ��� Ȯ��
            if (dialogueSequence.IsLastSegment)
            {
                // ���̾�α׿� ����Ʈ�� ��ϵǾ� �ִ� ���
                if (data.QuestData != null)
                {
                    // ����Ʈ�� ó�� ���� ���, �ڵ� ����
                    if (data.QuestData.IsFirst)
                    {
                        data.QuestData.IsFirst = false;

                        QuestController.Instance.AcceptQuest(data.QuestData);
                    }
                    else
                    {
                        List<ResponseContainer> contaienr = new List<ResponseContainer>();
                        contaienr.Add(new ResponseContainer(ResponseButtonType.Accept, () => QuestController.Instance.AcceptQuest(data.QuestData)));
                        contaienr.Add(new ResponseContainer(ResponseButtonType.Reject, () => QuestController.Instance.RejectQuest(data.QuestData)));

                        // ����Ʈ ���� �г��� ����
                        View.OpenResponsePanel(contaienr);

                        // Handler: �̺�Ʈ�� �߻����� �� ȣ��Ǵ� �Լ��� ��Ī�Ѵ� (������ ����)
                        var isClicked = false;
                        void ResponseHandler()
                        {
                            isClicked = true;
                            View.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
                            View.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
                        }
                        View.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
                        View.ResponsePanel.Accept.onClick.AddListener(ResponseHandler);
                        View.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
                        View.ResponsePanel.Reject.onClick.AddListener(ResponseHandler);

                        // �ش� ����Ʈ�� ���� / �����Ǳ� ������ ���
                        yield return new WaitUntil(() => isClicked);

                        // ����Ʈ ���� ���� ���� ���
                        SoundManager.Instance.PlayCommonSFX("SE_UI_Select");
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
        _currentDialogueData = null;
    }

    public void ShutdownDialogue()
    {
        if (_currentDialogueCoroutine == null)
        {
            Debug.Log("��ȭ�� ���� ���� �ƴմϴ�");
            return;
        }

        if (_currentDialogueData == null)
        {
            Debug.LogError("��ȭ�� ���� �������� ��ȭ �����Ͱ� �������� �ʽ��ϴ�");
            return;
        }

        SoundManager.Instance.PlayCommonSFX("SE_UI_Select");

        if (_currentDialogueData.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();

        StopCoroutine(_currentDialogueCoroutine);
        _currentDialogueCoroutine = null;
        _currentDialogueData = null;

        View.StopAllCoroutines();
        View.CleanUpOnSegmentOver();
        View.ClosePanel();

        IsDialogueActive = false;
    }
}
