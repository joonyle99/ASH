using System;
using UnityEngine;

public class SaveAndLoader : MonoBehaviour, ITriggerListener
{
    private string _passageName;
    public string PassageName => _passageName;

    private StatueVisualEffect _statueVisualEffect;

    public static Action OnSaveStarted;
    public static Action OnSaveEnded;

    private float _time = 3f;
    private float _timer = 0f;

    private void Awake()
    {
        _passageName = gameObject.name;
        _statueVisualEffect = GetComponent<StatueVisualEffect>();
        _timer = 0;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
    }

    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (_timer >= _time && activator.Type == ActivatorType.Player)
        {
            Save();
        }
    }

    public void Save()
    {
        if (_passageName == null) return;

        if(_statueVisualEffect && !_statueVisualEffect.Played)
        {
            PlayerBehaviour PB = SceneContext.Current.Player;
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