using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Rail : MonoBehaviour
{
    [SerializeField] SliderJoint2D _sliderJoint;
    [SerializeField] Transform[] _points;
    [SerializeField] int _currentSegmentID = 0;
    float _currentSegmentLength;
    // Update is called once per frame
    private void Awake()
    {
        SetSegmentID(0);
    }
    public void FixedUpdate()
    {
        if (_currentSegmentID > 0 && _sliderJoint.jointTranslation <= -_currentSegmentLength/2 + 0.001f)
            SetSegmentID(_currentSegmentID - 1);
        else if (_currentSegmentID < _points.Length - 1 && _sliderJoint.jointTranslation >= _currentSegmentLength/2 + 0.001f)
            SetSegmentID(_currentSegmentID + 1);
    }

    void SetSegmentID(int segmentID)
    {
        _currentSegmentID = segmentID;
        _sliderJoint.connectedAnchor = (_points[segmentID].position + _points[segmentID + 1].position) / 2f;
        _currentSegmentLength = Vector3.Distance(_points[segmentID].position, _points[segmentID + 1].position);
        var limits = new JointTranslationLimits2D();
        limits.max = _currentSegmentLength / 2;
        limits.min = -_currentSegmentLength / 2;
        _sliderJoint.limits = limits;
        _sliderJoint.angle = +180-Vector2.SignedAngle((_points[segmentID + 1].position - _points[segmentID].position), Vector2.right);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for(int i=0; i< _points.Length-1; i++)
            Gizmos.DrawLine(_points[i].position, _points[i+1].position);
    }
}
