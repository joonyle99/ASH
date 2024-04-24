using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다이얼로그 시퀀스
/// </summary>
public class Dialogue
{
    [Header("Dialogue")]
    [Space]

    private List<DialogueLine> _dialogueSequence;                           // 다이얼로그 콘텐츠
    private int _currentIndex;                                              // 현재 다이얼로그 인덱스

    public DialogueLine CurrentLine => _dialogueSequence[_currentIndex];    // 현재 다이얼로그 라인
    public bool IsOver => _currentIndex >= _dialogueSequence.Count;         // 다이얼로그가 끝났는지 여부

    public Dialogue(DialogueData data)
    {
        _dialogueSequence = data.GetDialogueSequence();
        _currentIndex = 0;
    }

    public DialogueLine MoveNext()
    {
        _currentIndex++;

        return IsOver ? null : _dialogueSequence[_currentIndex];
    }
}
