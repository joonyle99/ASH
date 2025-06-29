using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum FindDialogueDataType
{
    None = 0,
    Name = 1,
    Text = 2,
}

public class DialogueDataManager : HappyTools.SingletonBehaviourFixed<DialogueDataManager>
{
    [SerializeField]
    List<DialogueData> _dialogueDatas = new List<DialogueData>();

    string _groupName = "DialogueData";

    string _playAtFirstAdditionalKey = "_PlayAtFirst";
    string _playAtFirstAdditionalKeyForJson = "_PlayAtFirstSaved";

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
        ResetAllDialogueData();
        PersistentDataManager.TryAddDataGroup(Instance._groupName);

        string additionalKey_playAtFirst = isNeedJsonSave ?
            Instance._playAtFirstAdditionalKeyForJson : Instance._playAtFirstAdditionalKey;
        
        if (PersistentDataManager.Instance)
        {
            for(int i = 0; i < Instance._dialogueDatas.Count; i++)
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
}
