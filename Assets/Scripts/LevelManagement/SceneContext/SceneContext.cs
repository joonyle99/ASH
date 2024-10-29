using UnityEngine;
using System.Linq;
using Com.LuisPedroFonseca.ProCamera2D;

[System.Serializable]
public enum CameraType
{
    Normal,
    Chasing,
    Surrounding,
}

/// <summary>
/// ���� ���� ���� ������ ��� �����.
/// ���� '�ֿ� ������Ʈ'�� ���� ���۷����� ���� ����
/// </summary>
public class SceneContext : MonoBehaviour
{
    public static SceneContext Current { get; private set; }                // singleton

    public CameraType CameraType { get; set; } = CameraType.Normal;

    // basic
    public PlayerBehaviour Player { get; private set; }                     // �÷��̾�
    public ProCamera2D ProCamera { get; private set; }                      // ���� ī�޶�
    public CameraController CameraController { get; private set; }          // ī�޶� ��Ʈ�ѷ�

    // extra
    public Passage EntrancePassage { get; private set; }                                                                                // ���� �Ա�
    public SceneTransitionPlayer SceneTransitionPlayer { get; private set; }                                                            // �� ��ȯ �÷��̾� (Ÿ��Ʋ, ���ѷα׷��� �̵�)
    private PlayableSceneTransitionPlayer PlayableSceneTransitionPlayer => SceneTransitionPlayer as PlayableSceneTransitionPlayer;      // �� ��ȯ �÷��̾� (Ž�豸��, ��������, ������������ �̵�)

    [SerializeField] private CheckpointManager _checkpointManager;          // üũ����Ʈ �Ŵ���
    public CheckpointManager CheckPointManager => _checkpointManager;

    protected void Awake()
    {
        Current = this;
        ProCamera = FindObjectOfType<ProCamera2D>();
        CameraController = ProCamera.GetComponent<CameraController>();

        // SceneContext�� üũ����Ʈ �Ŵ����� �߰��Ѵ�
        if (_checkpointManager == null)
        {
            _checkpointManager = GetComponentInChildren<CheckpointManager>();

            if (_checkpointManager == null)
                _checkpointManager = gameObject.AddComponent<CheckpointManager>();
        }
    }

    /// <summary>
    /// �⺻������ ���ο� ���� Start���� ȣ��ǰ�, �߰��� NonPlayableScene, PlayableScene������ ��ȯ ��
    /// �ش� ���� ���� ���ؽ�Ʈ�� �����´�. i.e) �÷��̾�, �� ��ȯ �÷��̾�, ���� �Ա�, üũ ����Ʈ, ���ؽ�Ʈ ������ ...
    /// </summary>
    public Result BuildPlayable(string entranceName)
    {
        // Debug.Log($"call build playable in {name}");

        Result buildResult = Result.Success;

        // check build result
        void CheckBuildResult<T>(T target)
        {
            // local function

            if (target == null)
            {
                Debug.LogWarning($"<color=orange>[SceneContext Build]</color> There is No {typeof(T).Name} in scene !");

                if (buildResult == Result.Success)
                    buildResult = Result.Fail;
            }
        }

        // 1. player
        Player = FindFirstObjectByType<PlayerBehaviour>();

        // 2. scene transition player
        var playableTransitions = FindObjectsOfType<PlayableSceneTransitionPlayer>();   // ��� playable scene transition player�� ã�´�
        for (int index = 0; index < playableTransitions.Length; index++)
        {
            // Debug.Log($"Scene Transition Player: {playableTransitions[index].gameObject.name}");

            // scene change manager�� �پ����� ���� (�켱������ ����) scene transition player�� ã�Ƽ� �Ҵ����ش� (ex: Initial Scene Transition Player) 
            if (playableTransitions[index].GetComponent<SceneChangeManager>() == null)
            {
                // Debug.Log($"Scene Transition Player: {playableTransitions[index].gameObject.name}");

                SceneTransitionPlayer = playableTransitions[index];
                break;
            }
        }

        // 3. entrance
        var passages = FindObjectsByType<Passage>(FindObjectsSortMode.None);
        EntrancePassage = passages.ToList().Find(element => element.PassageName == entranceName);

        // check build result for player and entrance
        CheckBuildResult(Player);
        CheckBuildResult(EntrancePassage);

        // build check point
        Result checkpointBuildResult = _checkpointManager.CheckPointBuild();
        if (checkpointBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // build specific
        Result sceneSpecificBuildResult = SceneSpecificBuild();
        if (sceneSpecificBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // ** build default **
        Result defaultBuildResult = DefaultBuild();
        if (defaultBuildResult == Result.Fail)
            buildResult = Result.Fail;

        // Debug.Log($"SceneContext ������ ���� ���� ����: {buildResult == Result.Success}");

        return buildResult;
    }

    /// <summary>
    /// �� ���ؽ�Ʈ ���� �� �⺻������ ȣ��Ǵ� �Լ�.
    /// Ư���� �� ��ȯ �÷��̾ ���ٸ� �⺻ �� ��ȯ �÷��̾ ã�Ƽ� �������ش�.
    /// ���� ���� ��ȯ�Ǿ� �� ���ؽ�Ʈ�� ���� ����Ǹ�, ��� �����ʸ� ã�� �̺�Ʈ�� �����Ѵ�
    /// i.e) �� ��ȯ ��, ���� �Ա����� ������ �ƾ�, �÷��̾� ���� �ʱ�ȭ, ���� ���� UI ǥ�� ��
    /// </summary>
    public Result DefaultBuild()
    {
        Result buildResult = Result.Success;

        // �⺻ �� ��ȯ �÷��̾ �������ش�
        if (SceneTransitionPlayer == null)
        {
            SceneTransitionPlayer = FindFirstObjectByType<SceneTransitionPlayer>();

            if (SceneTransitionPlayer == null)
            {
                // Debug.LogError("There is No Any Scene Transition Player in scene ! (even SceneChangeManager)");
                buildResult = Result.Fail;
            }
        }

        // broadcast to all scene_context_build listener
        if (buildResult == Result.Success)
        {
            var monoBehaviours = FindObjectsOfType<MonoBehaviour>();
            var buildListeners = monoBehaviours.OfType<ISceneContextBuildListener>();

            foreach (var buildListener in buildListeners)
            {
                // Debug.Log($"SceneContextBuildListener: {name}");

                // �� ���ؽ�Ʈ ���� �Ϸ� �̺�Ʈ�� �����Ѵ�
                buildListener.OnSceneContextBuilt();
            }
        }

        return buildResult;
    }

    // extra
    protected virtual Result SceneSpecificBuild()
    {
        return Result.Success;
    }
    public void PlayerInstantRespawn()
    {
        PlayableSceneTransitionPlayer.PlayInstantRespawnEffect(_checkpointManager.LatestCheckpointPosition);
    }
}