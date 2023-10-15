using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    //TODO : checkpoint 탐색 및 추가를 compile time에 버튼 하나로 일괄적용 할 수 있게 해야함
    public Vector3 LatestCheckpointPosition { get; private set; }
    public void OnPlayPassedCheckpoint(Checkpoint checkpoint)
    {
        LatestCheckpointPosition = checkpoint.SpawnPosition;
    }

    public Result BuildPlayable()
    {
        Result buildResult = Result.Success;
        if (SceneContext.Current.Player != null)
            LatestCheckpointPosition = SceneContext.Current.Player.transform.position;
        else
            LatestCheckpointPosition = Vector3.zero;

        return buildResult;
    }

}