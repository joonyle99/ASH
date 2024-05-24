using UnityEngine;

public class RotationViewer : MonoBehaviour
{
    [Header("world rotation")]
    [Space]

    public Vector3 worldRotation;

    [Space]

    public float minWorldRotation = float.MaxValue;
    public float maxWorldRotation = float.MinValue;

    [Header("local rotation")]
    [Space]

    public Vector3 localRotation;

    [Space]

    public float minLocalRotation = float.MaxValue;
    public float maxLocalRotation = float.MinValue;

    [Header("player condition")]

    public bool isPlayer = false;

    public int recentDirFlag = 1;
    public StateBase stateFlag = new IdleState();

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            var player = SceneContext.Current.Player;
            var recentDir = player.RecentDir;
            var currentState = player.CurrentState;

            if (recentDirFlag != recentDir)
            {
                Debug.Log($"플레이어의 방향이 전환되었습니다 flag: {recentDirFlag} / recentDir: {recentDir}");

                recentDirFlag = recentDir;

                minWorldRotation = float.MaxValue;
                maxWorldRotation = float.MinValue;

                minLocalRotation = float.MaxValue;
                maxLocalRotation = float.MinValue;
            }
            else if (stateFlag != currentState)
            {
                Debug.Log($"플레이어의 상태가 전환되었습니다 flag: {stateFlag} / currentState: {currentState}");

                stateFlag = currentState;

                minWorldRotation = float.MaxValue;
                maxWorldRotation = float.MinValue;

                minLocalRotation = float.MaxValue;
                maxLocalRotation = float.MinValue;
            }
        }

        worldRotation = transform.rotation.eulerAngles;
        localRotation = transform.localRotation.eulerAngles;

        if (isPlayer)
        {
            if (worldRotation.z < minWorldRotation)
                minWorldRotation = worldRotation.z;
            if (worldRotation.z > maxWorldRotation)
                maxWorldRotation = worldRotation.z;

            if (localRotation.z < minLocalRotation)
                minLocalRotation = localRotation.z;
            if (localRotation.z > maxLocalRotation)
                maxLocalRotation = localRotation.z;
        }
    }
}
