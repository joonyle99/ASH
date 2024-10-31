using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : TriggerZone
{
    [Header("─────────── Passage ───────────")]
    [Space]

    [Tooltip("플레이어가 여기로 들어가서 다음 스테이지로 갈 때")][SerializeField] InputSetterScriptableObject _enterInputSetter;
    [Tooltip("플레이어가 이전 스테이지에서 여기로 나올 때")][SerializeField] InputSetterScriptableObject _exitInputSetter;

    [Space]

    [Tooltip("해당 입구를 통해 씬에 입성했을 때 실행하는 컷씬")]
    [SerializeField] private CutscenePlayer _entranceCutscenePlayer;

    [Space]

    [Tooltip("해당 입구를 통해 씬에 입성했을 때 플레이어가 스폰되는 위치")]
    [SerializeField] private Transform _playerSpawnPoint;

    [Space]

    [SerializeField] private bool _canEnter = true;
    [SerializeField] private float _exitTimeOut = 1f;

    private float BeforeCutsceneDelayTime
    {
        get
        {
            if(_entranceCutscenePlayer)
            {
                float delayTime = _entranceCutscenePlayer.IsPlayed ? 0f : 1f;
                return delayTime;
            }

            return 0f;
        }
    }

    public string PassageName => name;
    public InputSetterScriptableObject EnterInputSetter => _enterInputSetter;
    public InputSetterScriptableObject ExitInputSetter => _exitInputSetter;

    private bool _isPlayerExiting;

    void Awake()
    {
        if (_playerSpawnPoint == null)
            _playerSpawnPoint = this.transform;
    }

    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (_isPlayerExiting || !_canEnter)
            return;

        // 다음 씬으로 넘어간다
        StartCoroutine(ExitSceneCoroutine());
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        if (!_isPlayerExiting)
            return;

        _isPlayerExiting = false;
    }

    // 다음 씬으로 넘어가기 위한 로직
    private IEnumerator ExitSceneCoroutine()
    {
        // * push cutscene
        Cutscene exitSceneCutscene = new Cutscene(this, ExitSceneCutsceneCoroutine(), false);
        StartCoroutine(SceneEffectManager.Instance.PushCutscene(exitSceneCutscene));

        // # load next passage data
        var fromPassageName = name;
        var toPassageData = SceneChangeManager.Instance.GetNextPassageData(fromPassageName);          // this passage에 대응되는 next passage의 data를 가져온다
        var toSceneName = toPassageData.SceneName;

        // * wait cutscene
        yield return new WaitUntil(() => exitSceneCutscene.IsDone);

        // # change to next scene
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.ChangeMap;
        SceneChangeManager.Instance.ChangeToPlayableScene(toSceneName, toPassageData.PassageName);
    }
    private IEnumerator ExitSceneCutsceneCoroutine()
    {
        // 씬을 나가는 컷씬에서 플레이어의 입력을 받지 않도록 설정
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);

        // 씬을 나가는 효과
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
    }
    
    // Passage를 통해 밖으로 나옴
    public IEnumerator PlayerExitCoroutine()
    {
        _isPlayerExiting = true;

        // spawn point로 이동
        SceneContext.Current.Player.transform.position = _playerSpawnPoint.position;

        // exiting input setter
        if (_exitInputSetter != null) InputManager.Instance.ChangeInputSetter(_exitInputSetter);
        else InputManager.Instance.ChangeToDefaultSetter();

        // 아직 passage를 나가지 않았거나 시간이 지나지 않았다면 계속 대기
        float eTime = 0f;
        while(_isPlayerExiting || eTime < _exitTimeOut)
        {
            yield return null;
            eTime += Time.deltaTime;
        }
        yield return new WaitUntil(() => !_isPlayerExiting);
        yield return new WaitForSeconds(0.3f);

        // default input setter
        InputManager.Instance.ChangeToDefaultSetter();
    }

    // Passage에서 나온 후, 해당 씬에 컷씬을 실행
    public IEnumerator PlayEnterCutscene()
    {
        if(_entranceCutscenePlayer == null) yield break;
        if (!_entranceCutscenePlayer.isActiveAndEnabled) yield break;

        InputManager.Instance.ChangeToStayStillSetter();

        yield return new WaitForSeconds(BeforeCutsceneDelayTime);

        _entranceCutscenePlayer.Play();
    }
}
