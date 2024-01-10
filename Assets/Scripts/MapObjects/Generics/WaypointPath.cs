using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    Transform[] _wayPoints;
    [SerializeField] bool _loop = false;

    float[] _distances;

    float _totalDistance = 0;
    public float TotalDistance => _totalDistance;
    private void Awake()
    {
        var transforms = GetComponentsInChildren<Transform>();
        _wayPoints = new Transform[transforms.Length-1];
        for (int i = 0; i < transforms.Length - 1; i++)
            _wayPoints[i] = transforms[i + 1];

        _distances = new float[_wayPoints.Length - 1];
        for(int i=0; i<_wayPoints.Length-1; i++)
        {
            _distances[i] = Vector3.Distance(_wayPoints[i].position, _wayPoints[i + 1].position);
            _totalDistance += _distances[i];
        }
    }
    public Vector3 GetPosition(float travelDistance)
    {
        int idx = 0;
        int dir = 1;
        if (_loop)
            travelDistance = Mathf.Clamp(travelDistance, 0f, _totalDistance);
        while(travelDistance > 0)
        {
            if (travelDistance > _distances[idx])
            {
                travelDistance -= _distances[idx];
                if ((dir > 0 && idx == _distances.Length - 1) || (dir < 0 && idx == 0))
                    dir *= -1;
                else
                    idx += dir;
            }
            else
            {
                if (dir > 0)
                    return Vector3.Lerp(_wayPoints[idx].position, _wayPoints[idx + 1].position, travelDistance / _distances[idx]);
                else
                    return Vector3.Lerp(_wayPoints[idx].position, _wayPoints[idx + 1].position, 1- travelDistance / _distances[idx]);
            }
        }
        return _wayPoints[idx].position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        var transforms = GetComponentsInChildren<Transform>();
        for(int i=1; i< transforms.Length-1; i++)
        {
            Gizmos.DrawLine(transforms[i].position, transforms[i+1].position);
        }
    }
}
