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
        _teleportGraph1.gameObject.SetActive(true);
        _teleportGraph2.gameObject.SetActive(false);

        _currentGraph = _teleportGraph1;
    }
    public void ChangeGraphTo2()
    {
        _teleportGraph1.gameObject.SetActive(false);
        _teleportGraph2.gameObject.SetActive(true);

        _currentGraph = _teleportGraph2;
    }
}
