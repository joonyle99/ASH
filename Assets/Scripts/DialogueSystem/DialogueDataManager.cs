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
    /// PersistentDataManager�� ����� dialogueData������ ���� dialoguedata�� ����ȭ
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
    /// dialogueData�� PersistentDataManager�� ����
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

    public static List<DialogueData> FindDialogueDatas(FindDialogueDataType findDialogueDataType, string text)
    {
        List<DialogueData> result = new List<DialogueData>();

        switch (findDialogueDataType)
        {
            case FindDialogueDataType.Name:
                {
                    for (int i = 0; i < Instance._dialogueDatas.Count; i++)
                    {
                        if (Instance._dialogueDatas[i].name != "" && Instance._dialogueDatas[i].name.Contains(text))
                        {
                            result.Add(Instance._dialogueDatas[i]);
                        }
                    }

                    break;
                }
            case FindDialogueDataType.Text:
                {
                    for (int i = 0; i < Instance._dialogueDatas.Count; i++)
                    {
                        if (Instance._dialogueDatas[i].ScriptText != "" && Instance._dialogueDatas[i].ScriptText.Contains(text))
                        {
                            result.Add(Instance._dialogueDatas[i]);
                        }
                    }

                    break;
                }
        }

        return result;
    }
    public static DialogueData FindDialogueData(FindDialogueDataType findDialogueDataType, string text)
    {
        switch (findDialogueDataType)
        {
            case FindDialogueDataType.Name:
                {
                    for (int i = 0; i < Instance._dialogueDatas.Count; i++)
                    {
                        if (Instance._dialogueDatas[i].name != "" && Instance._dialogueDatas[i].name.Contains(text))
                        {
                            return Instance._dialogueDatas[i];
                        }
                    }

                    break;
                }
            case FindDialogueDataType.Text:
                {
                    for (int i = 0; i < Instance._dialogueDatas.Count; i++)
                    {
                        if (Instance._dialogueDatas[i].ScriptText != "" && Instance._dialogueDatas[i].ScriptText.Contains(text))
                        {
                            return Instance._dialogueDatas[i];
                        }
                    }

                    break;
                }
        }

        return null;
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
