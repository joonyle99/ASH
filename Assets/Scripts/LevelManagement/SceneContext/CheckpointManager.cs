using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // TODO : checkpoint Ž�� �� �߰��� compile time�� ��ư �ϳ��� �ϰ����� �� �� �ְ� �ؾ���
    public Vector3 LatestCheckpointPosition { get; private set; }
    public void OnPlayPassedCheckpoint(Checkpoint checkpoint)
    {
        LatestCheckpointPosition = checkpoint.SpawnPosition;
    }

    public Result CheckPointBuild()
    {
        Result buildResult = Result.Success;

        if (SceneContext.Current.Player != null)
            LatestCheckpointPosition = SceneContext.Current.Player.transform.position;
        else
            LatestCheckpointPosition = Vector3.zero;

        return buildResult;
    }
}