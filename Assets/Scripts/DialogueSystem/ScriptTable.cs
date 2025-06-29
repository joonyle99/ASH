using System.Collections.Generic;
using UnityEngine;

public static class ScriptTable
{
    private static Dictionary<int, Dictionary<string, string>> _tsvTable = new(); // { row key - column key } - value

    static ScriptTable()
    {
        ParseTSV();
    }
    private static void ParseTSV()
    {
        var tsvFile = Resources.Load<TextAsset>("Table/ScriptTable");
        string[] rows = tsvFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries); // a,,b -> [ a b ] / not [ a "" b ]

        if (rows.Length == 0)
            return;

        string[] columns = rows[0].Split('\t');
        for (int i = 1; i < rows.Length; ++i) // column key 제외하고 1번 인덱스부터 시작
        {
            string row = rows[i];
            string[] values = row.Split('\t');

            if (values.Length != columns.Length)
                continue;

            int rowKey = int.Parse(values[0]);
            var rowDict = new Dictionary<string, string>();

            for (int j = 1; j < columns.Length; ++j) // row key 제외하고 1번 인덱스부터 시작
            {
                string columnKey = columns[j];
                string value = values[j];
                value = value.Replace("\\n", "\n");
                rowDict[columnKey] = value;
            }

            _tsvTable[rowKey] = rowDict;
        }
    }
    public static string GetValue(int rowKey, string columnKey)
    {
        var value = _tsvTable[rowKey][columnKey];
        return value;
    }
    public static string GetSpeaker(int id)
    {
        // TODO: 예외 처리 필요 (id가 존재하지 않는 경우 등)
        if (_tsvTable.ContainsKey(id) == false)
        {
            throw new System.Exception($"ScriptTable에서 id {id}를 찾을 수 없습니다.");
        }

        int rowKey = id;
        string language = DialogueDataManager.Instance.GetLanguageStringCode();
        string columnKey = $"speaker_{language}";
        return _tsvTable[rowKey][columnKey];
    }
    public static string GetScript(int id)
    {
        // TODO: 예외 처리 필요 (id가 존재하지 않는 경우 등)
        if (_tsvTable.ContainsKey(id) == false)
        {
            throw new System.Exception($"ScriptTable에서 id {id}를 찾을 수 없습니다.");
        }

        int rowKey = id;
        string language = DialogueDataManager.Instance.GetLanguageStringCode();
        string columnKey = $"script_{language}";
        return _tsvTable[rowKey][columnKey];
    }
}
