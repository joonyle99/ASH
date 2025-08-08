using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Steamworks;
using System;

public enum LanguageCode
{
    KOREAN = 0,
    ENGLISH = 1, // DEFAULT LANGUAGE
    JAPANESE = 2,
}

public class DialogueDataManager : HappyTools.SingletonBehaviourFixed<DialogueDataManager>
{
    [SerializeField] private List<DialogueData> _dialogueDatas = new List<DialogueData>();
    [SerializeField] private LanguageCode _languageCode = LanguageCode.ENGLISH; // TODO: 로컬 데이터로 저장할 필요가 있음.. PlayerPrefs or Json..?

    string _groupName = "DialogueData";

    string _playAtFirstAdditionalKey = "_PlayAtFirst";
    string _playAtFirstAdditionalKeyForJson = "_PlayAtFirstSaved";

    public static UnityAction OnLanguageChanged;

    protected override void Awake()
    {
        Init();
    }

    private void Init()
    {
        _dialogueDatas = LoadAssetsOfType<DialogueData>().ToList();

        if (PersistentDataManager.Instance)
        {
            PersistentDataManager.TryAddDataGroup(_groupName);
        }

        SaveAndLoader.OnSaveStarted += SaveAllDialogueDataWithJson;
    }

    private void Start()
    {
        LoadLanguageCode();

        //// TODO: 스팀 언어를 가져온다

        //var steamLanguage = SteamApps.GetCurrentGameLanguage();

        //switch (steamLanguage)
        //{
        //    case "korean":
        //        _languageCode = LanguageCode.KOREAN;
        //        break;
        //    case "english":
        //        _languageCode = LanguageCode.ENGISH;
        //        break;
        //    case "japanese":
        //        _languageCode = LanguageCode.JAPANESE;
        //        break;
        //    default:
        //        _languageCode = LanguageCode.ENGISH; // 기본값으로 영어 사용
        //        break;
        //}
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        SaveAndLoader.OnSaveStarted -= SaveAllDialogueDataWithJson;
    }

    public static void ResetAllDialogueData()
    {
        for (int i = 0; i < Instance._dialogueDatas.Count; i++)
        {
            Instance._dialogueDatas[i].PlayAtFirst = true;
        }
    }

    /// <summary>
    /// PersistentDataManager에 저장된 dialogueData값들을 실제 dialoguedata에 동기화
    /// </summary>
    public static void LoadSyncAllDialogueData(bool isNeedJsonSave)
    {
        JsonDataManager.JsonLoad();
        ResetAllDialogueData();
        PersistentDataManager.TryAddDataGroup(Instance._groupName);

        string additionalKey_playAtFirst = isNeedJsonSave ?
            Instance._playAtFirstAdditionalKeyForJson : Instance._playAtFirstAdditionalKey;

        if (PersistentDataManager.Instance)
        {
            for (int i = 0; i < Instance._dialogueDatas.Count; i++)
            {
                bool playAtFirstSaveData = true;
                string id = Instance._dialogueDatas[i].name + additionalKey_playAtFirst;

                if (PersistentDataManager.Has<bool>(Instance._groupName, id))
                {
                    playAtFirstSaveData = PersistentDataManager.Get<bool>(Instance._groupName, id);
                }

                SetDialogueData(Instance._dialogueDatas[i], playAtFirstSaveData);
            }
        }
    }

    private static void SaveAllDialogueDataWithJson()
    {
        SaveAllDialogueData(true);
    }

    /// <summary>
    /// dialogueData를 PersistentDataManager에 저장
    /// </summary>
    public static void SaveAllDialogueData(bool isNeedJsonSave)
    {
        string additionalKey_playAtFirst = isNeedJsonSave ?
            Instance._playAtFirstAdditionalKeyForJson : Instance._playAtFirstAdditionalKey;

        if (PersistentDataManager.Instance)
        {
            for (int i = 0; i < Instance._dialogueDatas.Count; i++)
            {
                string id = Instance._dialogueDatas[i].name + additionalKey_playAtFirst;
                PersistentDataManager.Set(Instance._groupName, id, Instance._dialogueDatas[i].PlayAtFirst);
            }
        }
    }

    public static void SetDialogueData(DialogueData dialogueData, bool playAtFirst = true)
    {
        if (!dialogueData) return;

        dialogueData.PlayAtFirst = playAtFirst;
    }

    public static T[] LoadAssetsOfType<T>() where T : UnityEngine.Object
    {
        Resources.LoadAll("ScriptableObjects/DialogueData");
        return (T[])Resources.FindObjectsOfTypeAll(typeof(T));
    }

    public void SetLanguageCode(LanguageCode languageCode)
    {
        if (languageCode == _languageCode) return;

        _languageCode = languageCode;

        JsonDataManager.SaveLanguageCodeData(_languageCode.ToString());

        OnLanguageChanged?.Invoke();
    }
    public LanguageCode GetLanguageCode()
    {
        return _languageCode;
    }
    public string GetLanguageStringCode()
    {
        //Debug.Log(_languageCode);
        switch (_languageCode)
        {
            case LanguageCode.KOREAN:
                return "ko";
            case LanguageCode.ENGLISH:
                return "en";
            case LanguageCode.JAPANESE:
                return "ja";
            default:
                return "ko"; // 기본값으로 한국어 사용
        }
    }

    public void LoadLanguageCode()
    {
        string languageCode = "";

        if (JsonDataManager.Has("LanguageCode"))
        {
            JsonDataManager.LoadLanguageCodeData();

            languageCode = JsonDataManager.Instance.GlobalSaveData.saveDataGroup["LanguageCode"];
        }
        else
        {
            if (SteamManager.Initialized)
            {
                languageCode = SteamApps.GetCurrentGameLanguage().ToUpper();

                if (languageCode == "KOREANA")
                {
                    languageCode = "KOREAN";
                }
            }
            else
            {
                languageCode = "ENGLISH";
            }
        }

        switch (languageCode)
        {
            case "KOREAN":
                SetLanguageCode(LanguageCode.KOREAN);
                break;
            case "ENGLISH":
                SetLanguageCode(LanguageCode.ENGLISH);
                break;
            case "JAPANESE":
                SetLanguageCode(LanguageCode.JAPANESE);
                break;
        }
    }
}
