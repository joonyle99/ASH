using UnityEngine;

public class DirectionDetector : MonoBehaviour
{
    [SerializeField] int _defaultDir = 1;
    [SerializeField] int _recentDir;

    public int DefaultDir { get { return _defaultDir; } }
    public int RecentDir => _recentDir;

    private void Start()
    {
        _recentDir = _defaultDir;
    }
}
