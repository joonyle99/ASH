using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어가 밀어서 쓰러지는 나무
/// </summary>
public class FallingTreeByPush : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;
    [SerializeField] private Transform _forcePoint;

    [SerializeField] private float _power = 40f;
    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle;

    [SerializeField] private bool _isPushed;
    [SerializeField] private bool _isFalling;
    [SerializeField] private float _dir;

    private Rigidbody2D _rigid;

    private Quaternion _startRotation;
    private Quaternion _curRotation;

    private bool _isChangedLayer;

    public bool IsFalling { get { return _isFalling; } }
    [SerializeField] SoundList _soundList;

    bool _isFallingSoundPlayed = false;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        // set start rotation
        _startRotation = this.transform.rotation;
    }

    void Update()
    {
        // -------------------------------- //
        //          쓰러지는 타이밍          //
        // -------------------------------- //

        // update current rotation
        _curRotation = this.transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(_startRotation, _curRotation);

        // falling down tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
        {
            _isFalling = true;
            if (!_isFallingSoundPlayed)
            {
                _isFallingSoundPlayed = true;
                _soundList.PlaySFX("SE_FallingTree_Break");
            }
        }

        // 나무가 쓰러지는 타이밍에 레이어를 한번만 바꿔준다.
        if (_isFalling && !_isChangedLayer)
            ChangeLayer();
    }

    void FixedUpdate()
    {
        if (_isPushed)
            FallDown();
    }

    public void FallDown()
    {
        if (_isFalling)
            return;

        // 힘(N)을 입력하면 강체의 질량과 DT를 고려해서 속도를 변경한다.
        _rigid.AddForceAtPosition(Vector2.right * _dir * _power, _forcePoint.position, ForceMode2D.Force);
    }

    public void StartPush(float dir)
    {
        _rigid.constraints = RigidbodyConstraints2D.None;

        _isPushed = true;
        _dir = dir;
    }

    public void StopPush()
    {
        _isPushed = false;
        _dir = 0f;
    }

    private int ChangeToIndex(int v)
    {
        int value = 1;
        int index = 0;

        while (v != value)
        {
            value <<= 1;
            index++;
        }

        return index;
    }

    private void ChangeLayer()
    {
        Transform parent = this.transform.parent;

        /*
        parent.gameObject.layer = ChangeToIndex(_targetLayerMask.value);
        foreach (Transform child in parent)
            child.gameObject.layer = ChangeToIndex(_targetLayerMask.value);
        */

        parent.transform.GetChild(0).gameObject.layer = ChangeToIndex(_targetLayerMask.value);
        parent.transform.GetChild(1).gameObject.layer = ChangeToIndex(_targetLayerMask.value);

        _isChangedLayer = true;
    }
}
