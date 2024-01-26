using System.Collections;
using UnityEngine;

public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{

    [SerializeField] float _waitTimeAfterScriptEnd;
    DialogueView _view;
    DialogueView View
    {
        get
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>(true);
            return _view;
        }
    }
    public bool IsDialogueActive => View.IsPanelActive;
    public void StartDialogue(DialogueData data, bool isFromCutscene = false)
    {
        if (IsDialogueActive)
            return;
        StartCoroutine(DialogueCoroutine(data));
    }
    IEnumerator DialogueCoroutine(DialogueData data)
    {
        Dialogue dialogue = new Dialogue(data);
        //Disable Inputs
        if (data.InputSetter != null)
        {
            InputManager.Instance.ChangeInputSetter(data.InputSetter);
        }

        //Start Dialogue
        View.OpenPanel();

        //Proceed Dialogue
        while (!dialogue.IsOver)
        {
            View.StartSingleLine(dialogue.CurrentLine);
            while (!View.IsCurrentLineOver)
            {
                yield return null;
                if (InputManager.Instance.State.InteractionKey.KeyDown)
                    View.FastForward();
            }
            yield return null;
            yield return new WaitUntil(() => InputManager.Instance.State.InteractionKey.KeyDown);
            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
            yield return StartCoroutine(View.ClearTextCoroutine(_waitTimeAfterScriptEnd));
            dialogue.MoveNext();
        }

        //Close Dialogue
        View.ClosePanel();

        //Retain control
        if (data.InputSetter != null)
            InputManager.Instance.ChangeToDefaultSetter();
    }

}
