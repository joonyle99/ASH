using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다이얼로그 시퀀스
/// </summary>
public class DialogueSequence
{
    [Header("DialogueSequence")]
    [Space]

    private List<DialogueSegment> _dialogueSequence;                                // 다이얼로그 콘텐츠
    private int _currentIndex;                                                      // 현재 다이얼로그 인덱스

    public DialogueSegment CurrentSegment => _dialogueSequence[_currentIndex];      // 현재 다이얼로그 라인
    public bool IsLastSegment => _currentIndex == _dialogueSequence.Count - 1;      // 마지막 다이얼로그 세그먼트인지 여부
    public bool IsOver => _currentIndex >= _dialogueSequence.Count;                 // 다이얼로그가 끝났는지 여부

    public DialogueSequence(DialogueData data)
    {
        _dialogueSequence = data.GetDialogueSequence();
        _currentIndex = 0;
    }

    public DialogueSegment MoveNext()
    {
        _currentIndex++;

        if (IsOver)
            return null;

        return _dialogueSequence[_currentIndex];
    }
}
