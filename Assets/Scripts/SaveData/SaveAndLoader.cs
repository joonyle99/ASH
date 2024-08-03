using System;
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

    private StatueVisualEffect _statueVisualEffect;

    public static Action OnSaveStarted;
    public static Action OnSaveEnded;

    private void Awake()
    {
        _passageName = gameObject.name;
        _isChangeSceneByLoading = false;
        _statueVisualEffect = GetComponent<StatueVisualEffect>();
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            Debug.Log("Save");
            Save();

            if(_statueVisualEffect)
            {
                _statueVisualEffect.PlayEffectsOnSaveStarted();
                _statueVisualEffect.DeactiveSaveTextLogic();
            }
        }
    }

    public void Save()
    {
        if (_passageName == null) return;

        if(OnSaveStarted != null)
            OnSaveStarted.Invoke();

        JsonDataManager.SavePersistentData(_passageName);

        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        JsonDataManager.JsonSave();

        if(OnSaveEnded != null)
            OnSaveEnded.Invoke();
    }
}