using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Data", fileName = "New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] InputSetterScriptableObject _inputOverrider;
    [SerializeField] TextAsset _script;
    [SerializeField] List<DialogueAction> _actions;

    public string Name { get { return _name; } }
    public InputSetterScriptableObject InputOverrider { get { return _inputOverrider; } }
    public TextAsset Script { get { return _script; } }
    public List<DialogueAction> Actions { get { return _actions; } }

    public List<DialogueScriptInfo> GetScript()
    {
        List<DialogueScriptInfo> dialogueScriptInfos = new List<DialogueScriptInfo>();
        string [] scriptLines = HappyTools.TSVRead.SplitLines(_script.text);

        for (int i=0; i<scriptLines.Length; i++)
        {
            DialogueScriptInfo info = new DialogueScriptInfo();
            info.Text = scriptLines[i];
            dialogueScriptInfos.Add(info);
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
