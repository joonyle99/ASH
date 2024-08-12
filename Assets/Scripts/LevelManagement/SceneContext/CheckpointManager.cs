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
    /// �� ���ؽ�Ʈ ���� �� (�� ��ȯ ��) üũ ����Ʈ�� �����Ѵ�
    /// </summary>
    public Result CheckPointBuild()
    {
        Result buildResult = Result.Success;

        // �Ա��� �ִٸ� �Ա��� ��ġ�� üũ����Ʈ�� ����
        var entrancePassage = SceneContext.Current.EntrancePassage;
        if (entrancePassage)
        {
            // �Ա��� ���⿡ ���� üũ����Ʈ ��ġ�� ����
            var exitMoveInputSetter = entrancePassage.ExitInputSetter as MoveStraightInputSetter;
            if (exitMoveInputSetter)
            {
                var exitDirection = exitMoveInputSetter.direction == MoveStraightInputSetter.Direction.Right ? 1 : -1;
                var offset = Vector3.right * 3f * exitDirection;

                LatestCheckpointPosition = entrancePassage.transform.position + offset;
                return buildResult;
            }

            // �Ա��� ��ġ�� �� üũ����Ʈ ��ġ
            var exitStayInputSetter = entrancePassage.ExitInputSetter as StayStillInputSetter;
            if (exitStayInputSetter)
            {
                LatestCheckpointPosition = entrancePassage.transform.position;
                return buildResult;
            }
        }

        // �Ա��� ���ٸ� �÷��̾��� ��ġ�� üũ����Ʈ�� ����
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}