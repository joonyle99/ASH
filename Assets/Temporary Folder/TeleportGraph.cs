using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeleportGraph : MonoBehaviour
{
    [SerializeField] private Transform _subject;
    [SerializeField] private TeleportNode[] _teleportNodes;

    [Space]

    [SerializeField] private TeleportNode _initNode;

    [SerializeField] private TeleportNode _currentNode;
    [SerializeField] private TeleportNode _nextNode;

    private void Awake()
    {
        InitTeleportNode();
    }

    public void InitTeleportNode()
    {
        if (_initNode != null)
        {
            _currentNode = _initNode;
            _nextNode = _currentNode.GetRandomConnectedNode();
        }
        else if (_currentNode == null)
        {
            _currentNode = _teleportNodes[Random.Range(0, _teleportNodes.Length)];
            _nextNode = _currentNode.GetRandomConnectedNode();
        }
    }
    public void UpdateTeleportNode()
    {
        _currentNode = _nextNode;
        _nextNode = _currentNode.GetRandomConnectedNode();
    }

    public void Move()
    {
        if (_currentNode == null || _nextNode == null)
        {
            Debug.Log($"노드를 Move 중에 초기화합니다.");
            InitTeleportNode();
        }

        // move
        _subject.position = _currentNode.transform.position;

        // update
        UpdateTeleportNode();
    }
}
