using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : TriggerZone
{
    [Header("���������������������� Passage ����������������������")]
    [Space]

    [Tooltip("�÷��̾ ����� ���� ���� ���������� �� ��")][SerializeField] InputSetterScriptableObject _enterInputSetter;
    [Tooltip("�÷��̾ ���� ������������ ����� ���� ��")][SerializeField] InputSetterScriptableObject _exitInputSetter;

    [Space]

    [Tooltip("�ش� �Ա��� ���� ���� �Լ����� �� �����ϴ� �ƾ�")]
    [SerializeField] private CutscenePlayer _entranceCutscenePlayer;

    [Space]

    [Tooltip("�ش� �Ա��� ���� ���� �Լ����� �� �÷��̾ �����Ǵ� ��ġ")]
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

        // ���� ������ �Ѿ��
        StartCoroutine(ExitSceneCoroutine());
    }
    public override void OnPlayerExit(PlayerBehaviour player)
    {
        if (!_isPlayerExiting)
            return;

        _isPlayerExiting = false;
    }

    // ���� ������ �Ѿ�� ���� ����
    private IEnumerator ExitSceneCoroutine()
    {
        // * push cutscene
        Cutscene exitSceneCutscene = new Cutscene(this, ExitSceneCutsceneCoroutine(), false);
        StartCoroutine(SceneEffectManager.Instance.PushCutscene(exitSceneCutscene));

        // # load next passage data
        var fromPassageName = name;
        var toPassageData = SceneChangeManager.Instance.GetNextPassageData(fromPassageName);          // this passage�� �����Ǵ� next passage�� data�� �����´�
        var toSceneName = toPassageData.SceneName;

        // * wait cutscene
        yield return new WaitUntil(() => exitSceneCutscene.IsDone);

        // # change to next scene
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.ChangeMap;
        SceneChangeManager.Instance.ChangeToPlayableScene(toSceneName, toPassageData.PassageName);
    }
    private IEnumerator ExitSceneCutsceneCoroutine()
    {
        // ���� ������ �ƾ����� �÷��̾��� �Է��� ���� �ʵ��� ����
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);

        // ���� ������ ȿ��
        yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
    }
    
    // Passage�� ���� ������ ����
    public IEnumerator PlayerExitCoroutine()
    {
        _isPlayerExiting = true;

        // spawn point�� �̵�
        SceneContext.Current.Player.transform.position = _playerSpawnPoint.position;

        // exiting input setter
        if (_exitInputSetter != null) InputManager.Instance.ChangeInputSetter(_exitInputSetter);
        else InputManager.Instance.ChangeToDefaultSetter();

        // ���� passage�� ������ �ʾҰų� �ð��� ������ �ʾҴٸ� ��� ���
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

    // Passage���� ���� ��, �ش� ���� �ƾ��� ����
    public IEnumerator PlayEnterCutscene()
    {
        if(_entranceCutscenePlayer == null) yield break;
        if (!_entranceCutscenePlayer.isActiveAndEnabled) yield break;

        InputManager.Instance.ChangeToStayStillSetter();

        yield return new WaitForSeconds(BeforeCutsceneDelayTime);

        _entranceCutscenePlayer.Play();
    }
}
