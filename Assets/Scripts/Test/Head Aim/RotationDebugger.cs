using UnityEngine;

public class RotationDebugger : MonoBehaviour
{
    [Header("world rotation")]
    [Space]
    [Space]

    public Vector3 worldRotation;

    [Space]

    public float idleMinWorldRotation = float.MaxValue;
    public float idleMaxWorldRotation = float.MinValue;

    [Space]

    public float runMinWorldRotation = float.MaxValue;
    public float runMaxWorldRotation = float.MinValue;

    [Header("local rotation")]
    [Space]
    [Space]

    public Vector3 localRotation;

    [Space]

    public float idle_minLocalRotation = 0f;
    public float idle_maxLocalRotation = 0f;

    [Space]

    public float run_maxUpLocalRotation = 0f;
    public float run_maxDownLocalRotation = 0f;

    [Header("player condition")]

    public int recentDirFlag = 1;
    public StateBase stateFlag;

    void Update()
    {
        var player = SceneContext.Current.Player;
        var recentDir = player.RecentDir;

        worldRotation = transform.rotation.eulerAngles;

        if (player.CurrentStateIs<IdleState>())
        {
            if (idleMinWorldRotation > worldRotation.z)
                idleMinWorldRotation = worldRotation.z;
            if (idleMaxWorldRotation < worldRotation.z)
                idleMaxWorldRotation = worldRotation.z;
        }
        else if (player.CurrentStateIs<RunState>())
        {
            if (runMinWorldRotation > worldRotation.z)
                runMinWorldRotation = worldRotation.z;
            if (runMaxWorldRotation < worldRotation.z)
                runMaxWorldRotation = worldRotation.z;
        }
    }
}
