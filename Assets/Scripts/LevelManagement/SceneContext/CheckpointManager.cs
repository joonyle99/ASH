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
    /// 씬 컨텍스트 빌드 시 (씬 전환 시) 체크 포인트를 갱신한다
    /// </summary>
    public Result CheckPointBuild()
    {
        Result buildResult = Result.Success;

        // 입구가 있다면 입구의 위치를 체크포인트로 설정
        var entrancePassage = SceneContext.Current.EntrancePassage;
        if (entrancePassage)
        {
            // 입구의 방향에 따라 체크포인트 위치를 설정
            var exitMoveInputSetter = entrancePassage.ExitInputSetter as MoveStraightInputSetter;
            if (exitMoveInputSetter)
            {
                var exitDirection = exitMoveInputSetter.direction == MoveStraightInputSetter.Direction.Right ? 1 : -1;
                var offset = Vector3.right * 3f * exitDirection;

                LatestCheckpointPosition = entrancePassage.transform.position + offset;
                return buildResult;
            }

            // 입구의 위치가 곧 체크포인트 위치
            var exitStayInputSetter = entrancePassage.ExitInputSetter as StayStillInputSetter;
            if (exitStayInputSetter)
            {
                LatestCheckpointPosition = entrancePassage.transform.position;
                return buildResult;
            }
        }

        // 입구가 없다면 플레이어의 위치를 체크포인트로 설정
        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}