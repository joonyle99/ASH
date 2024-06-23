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

        /*
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player != null ? player.transform.position : Vector3.zero;
        */

        // �ʿ��ִ� �Ա��� ���ǵ�, �������� �������� ���������� �������� �Ա��� üũ�ϰ�
        // �������� ������ �Ա���� �Ա� ���� ���ʿ��� �������� �ϰ�
        // ���������� ������ �Ա���� �Ա� ���� �����ʿ��� �������� �Ѵ�

        // �Ա��� �ִٸ� �Ա��� ��ġ�� üũ����Ʈ�� ����
        var entrancePassage = SceneContext.Current.EntrancePassage;
        if (entrancePassage)
        {
            var exitInputSetter = entrancePassage.ExitInputSetter as MoveStraightInputSetter;
            if (exitInputSetter)
            {
                var exitDirection = exitInputSetter.direction == MoveStraightInputSetter.Direction.Right ? 1 : -1;
                var offset = Vector3.right * 3f * exitDirection;

                LatestCheckpointPosition = entrancePassage.transform.position + offset;
                return buildResult;
            }
        }

        // �Ա��� ���ٸ� �÷��̾��� ��ġ�� üũ����Ʈ�� ����
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}