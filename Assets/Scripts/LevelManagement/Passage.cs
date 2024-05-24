using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : TriggerZone
{
    [Tooltip("�÷��̾ ����� ���� ���� ���������� �� ��")][SerializeField] InputSetterScriptableObject _enterInputSetter;
    [Tooltip("�÷��̾ ���� ������������ ����� ���� ��")][SerializeField] InputSetterScriptableObject _exitInputSetter;

    [SerializeField] private Transform _playerSpawnPoint;

    public string PassageName => name;
    public InputSetterScriptableObject EnterInputSetter => _enterInputSetter;
    public InputSetterScriptableObject ExitInputSetter => _exitInputSetter;

    [SerializeField] private bool _canEnter = true;
    [SerializeField] private float _exitTimeOut = 1f;

    [SerializeField] private bool _isStartingPassage;
    public bool IsStartingPassage => _isStartingPassage;

    private bool _isPlayerExiting;

    void Awake()
    {
        if (_playerSpawnPoint == null)
            _playerSpawnPoint = transform;
    }

    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (_isPlayerExiting || !_canEnter)
            return;

        StartCoroutine(ExitSceneCoroutine());
    }

    IEnumerator ExitSceneCoroutine()
    {
        Cutscene _exitSceneCutscene = new Cutscene(this, ExitSceneCutsceneCoroutine(), false);
        SceneEffectManager.Current.PushCutscene(_exitSceneCutscene);

        var nextPassageData = SceneChangeManager.Instance.GetNextPassageData(name);
        string nextSceneName = nextPassageData.SceneName;

        yield return new WaitUntil(() => _exitSceneCutscene.IsDone);

        SceneChangeManager.Instance.ChangeToPlayableScene(nextSceneName, nextPassageData.PassageName);
    }
    IEnumerator ExitSceneCutsceneCoroutine()
    {
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        if (!_isPlayerExiting)
            return;
        _isPlayerExiting = false;
    }
    
    //Passage�� ���� ������ ����
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
        float eTime = 0f;
        while(_isPlayerExiting && eTime < _exitTimeOut)
        {
            yield return null;
            eTime += Time.deltaTime;
        }
        yield return new WaitUntil(() => !_isPlayerExiting);
        yield return new WaitForSeconds(0.3f);
        InputManager.Instance.ChangeToDefaultSetter();
    }
}
