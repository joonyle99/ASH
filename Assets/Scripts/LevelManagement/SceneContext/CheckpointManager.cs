using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // TODO : checkpoint Ž�� �� �߰��� compile time�� ��ư �ϳ��� �ϰ����� �� �� �ְ� �ؾ���

    [field: SerializeField]
    public Vector3 LatestCheckpointPosition { get; private set; }

    public void OnPlayPassedCheckpoint(Checkpoint checkpoint)
    {
        LatestCheckpointPosition = checkpoint.SpawnPosition;
    }

    /// <summary>
    /// �� ���ؽ�Ʈ ���� �� ȣ��Ǵ� �Լ�.
    /// üũ ����Ʈ�� �����Ѵ�
    /// </summary>
    /// <returns></returns>
    public Result CheckPointBuild()
    {
        Result buildResult = Result.Success;

        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player != null ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}