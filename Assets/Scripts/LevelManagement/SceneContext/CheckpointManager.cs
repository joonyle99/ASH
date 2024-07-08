using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    // TODO : checkpoint 탐색 및 추가를 compile time에 버튼 하나로 일괄적용 할 수 있게 해야함

    [field: SerializeField]
    public Vector3 LatestCheckpointPosition { get; private set; }

    public void OnPlayPassedCheckpoint(Checkpoint checkpoint)
    {
        LatestCheckpointPosition = checkpoint.SpawnPosition;
    }

    /// <summary>
    /// 씬 컨텍스트 빌드 시 호출되는 함수.
    /// 체크 포인트를 갱신한다
    /// </summary>
    /// <returns></returns>
    public Result CheckPointBuild()
    {
        Result buildResult = Result.Success;

        /*
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player != null ? player.transform.position : Vector3.zero;
        */

        // 맵에있는 입구로 갈건데, 왼쪽으로 나가는지 오른쪽으로 나가는지 입구를 체크하고
        // 왼쪽으로 나가는 입구라면 입구 보다 왼쪽에서 나오도록 하고
        // 오른쪽으로 나가는 입구라면 입구 보다 오른쪽에서 나오도록 한다

        // 입구가 있다면 입구의 위치를 체크포인트로 설정
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

        // 입구가 없다면 플레이어의 위치를 체크포인트로 설정
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}