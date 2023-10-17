using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : ITriggerZone
{
    [Tooltip("플레이어가 여기로 들어가서 다음 스테이지로 갈 때")][SerializeField] InputSetterScriptableObject _enterInputSetter;
    [Tooltip("플레이어가 이전 스테이지에서 여기로 나올 때")][SerializeField] InputSetterScriptableObject _exitInputSetter;

    [SerializeField] Transform _playerSpawnPoint;

    public string PassageName => name;
    public InputSetterScriptableObject EnterInputSetter => _enterInputSetter;
    public InputSetterScriptableObject ExitInputSetter => _exitInputSetter;


    bool _isPlayerExiting;
    void Awake()
    {
        if (_playerSpawnPoint == null)
            _playerSpawnPoint = transform;
    }
    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (_isPlayerExiting)
            return;
        StartCoroutine(ExitSceneCoroutine());
    }
    IEnumerator ExitSceneCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
        yield return SceneContext.Current.SceneTransitionPlayer.ExitEffectCoroutine();
        var nextPassageData = SceneChangeManager.Instance.GetNextPassageData(name);
        string nextSceneName = nextPassageData.SceneName;
        SceneChangeManager.Instance.ChangeToPlayableScene(nextSceneName, nextPassageData.PassageName);
    }
    public override void OnActivatorExit(TriggerActivator activator)
    {
        if (!_isPlayerExiting)
            return;
        if (activator.IsPlayer)
            _isPlayerExiting = false;
    }
    
    //Passage를 통해 밖으로 나옴
    public IEnumerator PlayerExitCoroutine()
    {
        //Spawn player
        _isPlayerExiting = true;
        SceneContext.Current.Player.transform.position = _playerSpawnPoint.position;
        if (_exitInputSetter != null)
            InputManager.Instance.ChangeInputSetter(_exitInputSetter);
        else
            InputManager.Instance.ChangeToDefaultSetter();

        //Wait until player exits zone
        yield return new WaitUntil(() => !_isPlayerExiting);
        yield return new WaitForSeconds(0.3f);
        InputManager.Instance.ChangeToDefaultSetter();
    }
}
