using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Data", fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] InputSetterScriptableObject _inputOverrider;
    [SerializeField] TextAsset _script;
    [SerializeField] float _defaultSpeed = 12;
    [SerializeField] List<DialogueAction> _actions;

    public string Name { get { return _name; } }
    public InputSetterScriptableObject InputOverrider { get { return _inputOverrider; } }
    public TextAsset Script { get { return _script; } }
    public List<DialogueAction> Actions { get { return _actions; } }

    public List<DialogueScriptInfo> GetScript()
    {
        List<DialogueScriptInfo> dialogueScriptInfos = new List<DialogueScriptInfo>();
        string [] scriptLines = HappyTools.TSVRead.SplitLines(_script.text);
        float speed = _defaultSpeed;
        ShakeInfo shake = ShakeInfo.None;
        for (int i=0; i<scriptLines.Length; i++)
        {
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
                        if (!float.TryParse(words[1].Trim(), out speed))
                            Debug.LogError("Can't parse speed in line " + i.ToString());
                    }
                    else if (words[0].Trim().ToLower() == "shake")
                    {
                        var shakeParams = words[1].Trim().Split('/');
                        Debug.Assert(shakeParams.Length == 3, "Can't parse shake in line " + i.ToString());
                        if (!float.TryParse(shakeParams[0].Trim(), out shake.RotationPower))
                            Debug.LogError("Can't parse shake in line " + i.ToString());
                        if (!float.TryParse(shakeParams[1].Trim(), out shake.MovePower))
                            Debug.LogError("Can't parse shake in line " + i.ToString());
                        if (!float.TryParse(shakeParams[2].Trim(), out shake.Speed))
                            Debug.LogError("Can't parse shake in line " + i.ToString());
                    }
                }
                continue;
            }
            DialogueScriptInfo info = new DialogueScriptInfo();
            info.Text = scriptLines[i];
            info.Speed = speed;
            info.Shake = shake;
            dialogueScriptInfos.Add(info);

            shake = ShakeInfo.None;
            speed = _defaultSpeed;
        }
        return dialogueScriptInfos;
    }
    /*
     * 대화정보: 
 -스크립트 (텍스트 line by line, @Action1 같은 것으로 특수동작 가능)
 -텍스트 style (default 색 등)
 -액션 (Action1, Action2에 해당하는 액션들 (화면흔들림, 선택지부여, 체력회복 등 뭐든 가능하게) 이건 List of scriptableobjects
 -다이얼로그에 필요한 정보 (이름, 프사 등)

     */
}
