using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 다이얼로그에 대한 모든 정보를 담는 ScriptableObject
/// </summary>
[CreateAssetMenu(menuName = "Dialogue Data", fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Data")]
    [Space]

    [SerializeField] private string _defaultSpeaker;                        // 대사 캐릭터 이름
    [SerializeField] private TextAsset _script;                             // 대사 스크립트
    [SerializeField] private float _typingSpeed = 12.5f;                    // 글자 속도
    [SerializeField] private InputSetterScriptableObject _inputSetter;      // 플레이어 입력 설정

    [SerializeField] public bool PlayAtFirst = true;

    [Space]

    [SerializeField] private DialogueAction[] _actions;                     // 대화 액션

    [SerializeField, TextArea(3, 30)] private string _scriptText;
    public string ScriptText => _scriptText;

    private Quest _quest;
    public Quest Quest
    {
        get => _quest;
        set => _quest = value;
    }

    public InputSetterScriptableObject InputSetter => _inputSetter;

    private void OnValidate()
    {
        if (_script)
        {
            _scriptText = _script.text;
        }
    }

    /// <summary>
    /// Quest를 DialogueData에 연결한다
    /// </summary>
    public void LinkQuestData(Quest quest)
    {
        Debug.Log("다이얼로그에 퀘스트 연결 성공");
        this.Quest = quest;
    }
    public void UnlinkQuestData()
    {
        Debug.Log("다이얼로그에서 퀘스트 해제 성공");
        this.Quest = null;
    }

    /// <summary>
    /// 다이얼로그 세그먼트 처리를 통해 시퀀스를 반환한다
    /// </summary>
    public List<DialogueSegment> GetDialogueSequence()
    {
        List<DialogueSegment> dialogueSequence = new List<DialogueSegment>();
        string [] scriptLines = HappyTools.TSVRead.SplitLines(_script.text);
        string speakerName = _defaultSpeaker;

        for (int i=0; i<scriptLines.Length; i++)
        {
            float charactersPerSecond = _typingSpeed;
            TextShakeParams shakeParams = TextShakeParams.None;
            if (scriptLines[i].Trim().Length == 0)
                continue;
            if (scriptLines[i].StartsWith("#"))
            {
                string[] commands = scriptLines[i].Split("#");
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
                        speakerName = words[1].Trim();
                    }
                }
                continue;
            }

            // TODO : @로 시작하는 특수 액션들에 대한 처리

            DialogueSegment segment = new DialogueSegment();
            segment.Text = scriptLines[i];
            segment.CharactersPerSecond = charactersPerSecond;
            segment.ShakeParams = shakeParams;
            segment.Speaker = speakerName;

            dialogueSequence.Add(segment);
        }
        return dialogueSequence;
    }

    public void SetDialogueData(bool playAtFirst)
    {
        PlayAtFirst = playAtFirst;
    }
}