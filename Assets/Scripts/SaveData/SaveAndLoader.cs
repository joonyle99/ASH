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
            if(_statueVisualEffect)
            {
                _statueVisualEffect.PlayEffectsOnSaveStarted();
                _statueVisualEffect.DeactiveSaveTextLogic();
            }

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

        JsonDataManager.JsonSave();

        if(OnSaveEnded != null)
            OnSaveEnded.Invoke();
    }
}