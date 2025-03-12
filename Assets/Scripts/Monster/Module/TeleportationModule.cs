using UnityEngine;

public class TeleportationModule : MonoBehaviour
{
    [SerializeField] private GameObject _firePrefab;

    [SerializeField] private TeleportGraph _teleportGraph1;
    [SerializeField] private TeleportGraph _teleportGraph2;

    [SerializeField] private TeleportGraph _currentGraph;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ExecuteTeleport_AnimEvent()
    {
        //var playerPos = SceneContext.Current.Player.transform.position;
        //var firePrefabPos = _firePrefab.transform.position;
        //_firePrefab.transform.position = new Vector3(firePrefabPos.x, playerPos.y, firePrefabPos.z);
        _currentGraph.Move(_rigidbody);
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
