using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : HappyTools.SingletonBehaviourFixed<DialogueManager>
{
    DialogueView View 
    {
        get 
        {
            if (_view == null)
                _view = FindObjectOfType<DialogueView>();
            return _view;
        }
    }
    DialogueView _view;

    bool _isDialogueActive = false;
    public void StartDialogue(DialogueData data)
    {
        if (_isDialogueActive)
            return;
        StartCoroutine(DialogueCoroutine(data));
    }

    IEnumerator DialogueCoroutine(DialogueData data)
    {
        _isDialogueActive = true;
        List<DialogueScriptInfo> scriptInfos = data.GetScript();
        //Disable Inputs
        InputManager.Instance.ChangeInputSetter(data.InputOverrider);
        //Start Dialogue
        View.OpenPanel();
        //Proceed Dialogue
        for (int i=0; i<scriptInfos.Count; i++)
        {
            yield return null;
            View.SetText(scriptInfos[i].Text);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        //Close Dialogue
        View.ClosePanel();
        //Retain control
        InputManager.Instance.ChangeToDefaultSetter();
        yield return null;
        _isDialogueActive = false;
    }

}
