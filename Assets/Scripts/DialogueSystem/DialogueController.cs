using System.Collections;
using UnityEngine;

public class DialogueController : HappyTools.SingletonBehaviourFixed<DialogueController>
{

    [SerializeField] float _waitTimeAfterScriptEnd;
    bool _isDialogueActive = false;
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

    public void StartDialogue(DialogueData data)
    {
        if (_isDialogueActive)
            return;
        StartCoroutine(DialogueCoroutine(data));
    }
    IEnumerator DialogueCoroutine(DialogueData data)
    {
        Dialogue dialogue = new Dialogue(data);
        //Disable Inputs
        if (data.InputSetter != null)
            InputManager.Instance.ChangeInputSetter(data.InputSetter);

        //Start Dialogue
        View.OpenPanel();

        //Proceed Dialogue
        while (!dialogue.IsOver)
        {
            View.StartSingleLine(dialogue.CurrentLine);
            while(!View.IsCurrentLineOver)
            {
                if (InputManager.InteractionKeyDown)
                    View.FastForward();
                yield return null;
            }
            yield return new WaitUntil(() => InputManager.InteractionKeyDown);
            SoundManager.Instance.PlayCommonSFXPitched("SE_UI_Select");
            yield return StartCoroutine(View.ClearTextCoroutine(_waitTimeAfterScriptEnd));

            dialogue.MoveNext();
        }

        //Close Dialogue
        View.ClosePanel();

        //Retain control
        InputManager.Instance.ChangeToDefaultSetter();

        _isDialogueActive = false;
    }

}
