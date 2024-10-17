using UnityEngine;

public class TeleportationModule : MonoBehaviour
{
    [SerializeField] private TeleportGraph _teleportGraph1;
    [SerializeField] private TeleportGraph _teleportGraph2;

    [SerializeField] private TeleportGraph _currentGraph;

    public void ExecuteTeleport_AnimEvent()
    {
        _currentGraph.Move();
    }

    public void ChangeGraphTo1()
    {
        _currentGraph = _teleportGraph1;
    }
    public void ChangeGraphTo2()
    {
        _currentGraph = _teleportGraph2;
    }
}
