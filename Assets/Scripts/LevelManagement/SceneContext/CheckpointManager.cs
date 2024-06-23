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

        var player = SceneContext.Current.Player;
        LatestCheckpointPosition = player != null ? player.transform.position : Vector3.zero;

        return buildResult;
    }
}