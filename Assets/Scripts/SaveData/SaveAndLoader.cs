using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SaveAndLoader : MonoBehaviour, ITriggerListener
{
    private string _passageName;
    public string PassageName => _passageName;

    private static bool _isChangeSceneByLoading = false;
    public static bool IsChangeSceneByLoading
    {
        get => _isChangeSceneByLoading;
        set => _isChangeSceneByLoading = value;
    }


    public static Action OnSaveStarted;
    public static Action OnSaveEnded;

    private void Awake()
    {
        _passageName = gameObject.name;
        _isChangeSceneByLoading = false;
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            Debug.Log("Save");
            Save();
        }
    }

    public void Save()
    {
        if (_passageName == null) return;

        if(OnSaveStarted != null)
            OnSaveStarted.Invoke();

        JsonDataManager.SavePersistentData(_passageName);

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if(Player != null)
        {
            PlayerBehaviour PB = Player.GetComponent<PlayerBehaviour>();
            JsonDataManager.SavePlayerData(new JsonPlayerData(PB.MaxHp, PB.CurHp));
        }

        JsonDataManager.JsonSave();

        if(OnSaveEnded != null)
            OnSaveEnded.Invoke();
    }

    
}