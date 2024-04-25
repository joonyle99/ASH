using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다이얼로그 시퀀스
/// </summary>
public class DialogueSequence
{
    [Header("Dialogue Sequence")]
    [Space]

    private List<DialogueSegment> _dialogueSequence;                                        // 다이얼로그 시퀀스
    private int _currentSegmentIndex;                                                       // 다이얼로그 세그먼트 인덱스

    public DialogueSegment CurrentSegment => _dialogueSequence[_currentSegmentIndex];       // 현재 다이얼로그 세그먼트
    public bool IsLastSegment => _currentSegmentIndex == _dialogueSequence.Count - 1;       // 마지막 다이얼로그 세그먼트인지 여부
    public bool IsOver => _currentSegmentIndex >= _dialogueSequence.Count;                  // 다이얼로그가 시퀀스가 끝났는지 여부

    public DialogueSequence(DialogueData data)
    {
        _dialogueSequence = data.GetDialogueSequence();
        _currentSegmentIndex = 0;
    }

    public DialogueSegment MoveNext()
    {
        _currentSegmentIndex++;

        if (IsOver)
            return null;

        return _dialogueSequence[_currentSegmentIndex];
    }
}
