using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    List<DialogueLine> _dialogueSequence;
    int _currentIndex;
    public bool IsOver { get { return _currentIndex >= _dialogueSequence.Count; } }
    public DialogueLine CurrentLine => _dialogueSequence[_currentIndex];

    public Dialogue(DialogueData data)
    {
        _dialogueSequence = data.GetDialogueSequence();
        _currentIndex = 0;
    }

    public DialogueLine MoveNext()
    {
        _currentIndex++;
        if (IsOver)
            return null;
        return _dialogueSequence[_currentIndex];
    }
}
