using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Icons;

[RequireComponent(typeof(TMP_Text))]
public class UITranslator : MonoBehaviour
{
    [SerializeField] private string _languageTableId;

    private static string s_textTranslateAssetPath = "Table/UILanguageTable";

    private TMP_Text textUI;

    private void Awake()
    {
        textUI = GetComponent<TMP_Text>();

        DialogueDataManager.OnLanguageChanged += ApplyLanguageChange;
    }

    public void ApplyLanguageChange()
    {
        string text = GetLocalizedString(_languageTableId);
        Debug.Log($"Ui language change to /{text}/");
        if (text != "")
        {
            textUI.text = text;
        }
    }

    public static string GetLocalizedString(string textId)
    {
        // 1. CSV 로드
        TextAsset csvFile = Resources.Load<TextAsset>(s_textTranslateAssetPath);
        if (csvFile == null)
        {
            Debug.LogError("Localization.csv not found in Resources.");
            return "";
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length < 2) return "";

        // 헤더 처리
        string[] headers = lines[0]
                            .Trim()
                            .Split('\t')
                            .Select(h => h.Trim())
                            .Where(h => !string.IsNullOrEmpty(h)) // 빈 항목 제거
                            .ToArray();
        string langCode = DialogueDataManager.Instance.GetLanguageStringCode().Trim();
        int langIndex = System.Array.IndexOf(headers, langCode);
        if (langIndex == -1) return "";

        // 3. 한 줄씩 탐색하며 ID에 해당하는 값 찾기
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line
                            .Trim()
                            .Split('\t')
                            .Select(h => h.Trim())
                            .Where(h => !string.IsNullOrEmpty(h)) // 빈 항목 제거
                            .ToArray();

            string rowId = columns[0].Trim();
            if (rowId == textId)
            {
                return columns[langIndex].Trim();
            }
        }

        return "";
    }
}
