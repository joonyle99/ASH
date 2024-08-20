using System;
using UnityEngine;

public class SaveAndLoader : MonoBehaviour, ITriggerListener
{
    private string _passageName;
    public string PassageName => _passageName;

    private StatueVisualEffect _statueVisualEffect;

    public static Action OnSaveStarted;
    public static Action OnSaveEnded;

    private void Awake()
    {
        _passageName = gameObject.name;
        _statueVisualEffect = GetComponent<StatueVisualEffect>();
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            Save();
        }
    }

    public void Save()
    {
        if (_passageName == null) return;

        if(_statueVisualEffect && !_statueVisualEffect.Played)
        {
            PlayerBehaviour PB = SceneContext.Current.Player.GetComponent<PlayerBehaviour>();
            PB?.RecoverCurHp(1);
        }

        _statueVisualEffect.PlayEffectsOnSaveStarted();
        _statueVisualEffect.DeactiveSaveTextLogic();

        OnSaveStarted?.Invoke();

        JsonDataManager.SavePersistentData(_passageName);

        JsonDataManager.JsonSave();

        OnSaveEnded?.Invoke();
    }
}