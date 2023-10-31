using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 다이얼로그에 대한 모든 정보를 담는 ScriptableObject.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue Data", fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] string _NPCName;
    [SerializeField] InputSetterScriptableObject _inputSetter;
    [SerializeField] TextAsset _script;
    [SerializeField] float _defaultCharactersPerSecond = 12;
    [SerializeField] DialogueAction[] _actions;

    public string NPCName => _NPCName;
    public InputSetterScriptableObject InputSetter => _inputSetter;
    public List<DialogueLine> GetDialogueSequence()
    {
        List<DialogueLine> dialogueSequence = new List<DialogueLine>();
        string [] scriptLines = HappyTools.TSVRead.SplitLines(_script.text);
        for (int i=0; i<scriptLines.Length; i++)
        {
            float charactersPerSecond = _defaultCharactersPerSecond;
            TextShakeParams shakeParams = TextShakeParams.None;
            if (scriptLines[i].Trim().Length == 0)
                continue;
            if (scriptLines[i].StartsWith("#"))
            {
                string[] commands = scriptLines[i].Split("#");
                for (int c = 0; c < commands.Length; c++) 
                {
                    var words = commands[c].Split(":");
                    if (words[0].Trim().ToLower() == "speed")
                    {
                        if (!float.TryParse(words[1].Trim(), out charactersPerSecond))
                            Debug.LogError("Can't parse speed in line " + i.ToString());
                    }
                    else if (words[0].Trim().ToLower() == "shake")
                    {
                        var shakeParamValues = words[1].Trim().Split('/');
                        Debug.Assert(shakeParamValues.Length == 3, "Can't parse shake in line " + i.ToString());
                        if (!float.TryParse(shakeParamValues[0].Trim(), out shakeParams.RotationPower)
                            || !float.TryParse(shakeParamValues[1].Trim(), out shakeParams.MovePower)
                            || !float.TryParse(shakeParamValues[2].Trim(), out shakeParams.Speed))
                            Debug.LogError("Can't parse shake in line " + i.ToString());
                    }
                }
                continue;
            }
            // TODO : @로 시작하는 특수 액션들에 대한 처리

            DialogueLine line = new DialogueLine();
            line.Text = scriptLines[i];
            line.CharactersPerSecond = charactersPerSecond;
            line.ShakeParams = shakeParams;

            dialogueSequence.Add(line);
        }
        return dialogueSequence;
    }
}