using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 다이얼로그에 대한 모든 정보를 담는 ScriptableObject
/// CF) DialogueSequence는 DialogueSegment의 집합
/// </summary>
[CreateAssetMenu(menuName = "Dialogue Data", fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Data")]
    [Space]

    [SerializeField] private int _scriptID;
    public int ScriptID => _scriptID;                                       // 스크립트 ID

    [Space]

    [Header("삭제 예정인 필드 - 대사 캐릭터, 대사 스크립트 (dialogue id로 대체)")]
    [SerializeField] private string _defaultSpeaker;
    [SerializeField] private TextAsset _script;

    [Space]

    [SerializeField] private InputSetterScriptableObject _inputSetter;      // 플레이어 키 입력 설정
    public InputSetterScriptableObject InputSetter => _inputSetter;

    [Space]

    public bool PlayAtFirst;                                                // 이미 한 번 본 경우, 다시 재생하지 않음
    private float _typingSpeed = 25f;                                       // 타이핑 속도

    private Quest _quest;                                                   // 다이얼로그에 연결된 퀘스트
    public Quest Quest
    {
        get => _quest;
        set => _quest = value;
    }

    private void OnEnable()
    {
        _quest = null;
    }

    /// <summary>
    /// Quest를 DialogueData에 연결한다
    /// </summary>
    public void LinkQuestData(Quest quest)
    {
        Debug.Log("[Success] - link quest data to dialogue data");
        this.Quest = quest;
    }
    public void UnlinkQuestData()
    {
        Debug.Log("[Failure] - link quest data to dialogue data");
        this.Quest = null;
    }

    /// <summary>
    /// 다이얼로그 세그먼트 처리를 통해 시퀀스를 반환한다
    /// </summary>
    public List<DialogueSegment> GetDialogueSequence()
    {
        string script = ScriptTable.GetScript(ScriptID);
        string speaker = ScriptTable.GetSpeaker(ScriptID);

        List<DialogueSegment> dialogueSequence = new List<DialogueSegment>();
        string [] lines = HappyTools.TSVRead.SplitLines(_script.text);

        for (int i=0; i<lines.Length; i++)
        {
            float charactersPerSecond = _typingSpeed;
            TextShakeParams shakeParams = TextShakeParams.None;
            if (lines[i].Trim().Length == 0)
                continue;
            if (lines[i].StartsWith("#"))
            {
                string[] commands = lines[i].Split("#");
                for (int c = 0; c < commands.Length; c++) 
                {
                    var words = commands[c].Split(":");
                    Debug.Log("index : " + c + " word : " + words);
                    var firstWord = words[0].Trim().ToLower();
                    if (firstWord == "speed")
                    {
                        if (!float.TryParse(words[1].Trim(), out charactersPerSecond))
                            Debug.LogError("Can't parse speed in line " + i.ToString());
                    }
                    else if (firstWord == "shake")
                    {
                        var shakeParamValues = words[1].Trim().Split('/');
                        Debug.Assert(shakeParamValues.Length == 3, "Can't parse shake in line " + i.ToString());
                        if (!float.TryParse(shakeParamValues[0].Trim(), out shakeParams.RotationPower)
                            || !float.TryParse(shakeParamValues[1].Trim(), out shakeParams.MovePower)
                            || !float.TryParse(shakeParamValues[2].Trim(), out shakeParams.Speed))
                            Debug.LogError("Can't parse shake in line " + i.ToString());
                    }
                    else if (firstWord == "name")
                    {
                        speaker = words[1].Trim();
                    }
                }
                continue;
            }

            DialogueSegment segment = new DialogueSegment();
            segment.Text = lines[i];
            segment.CharactersPerSecond = charactersPerSecond;
            segment.ShakeParams = shakeParams;
            segment.Speaker = _defaultSpeaker;

            dialogueSequence.Add(segment);
        }
        return dialogueSequence;
    }

    public void SetDialogueData(bool playAtFirst)
    {
        PlayAtFirst = playAtFirst;
    }
}
