using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 굴러오는 돌에 의해 파괴되며 쓰러지는 나무
/// </summary>
public class FallingTreeByCrash : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;

    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle;
    [SerializeField] private bool _isCrashed;

    private bool _isChangedLayer;

    private Rigidbody2D _rigid;

    private Quaternion _startRotation;
    private Quaternion _curRotation;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        _startRotation = this.transform.rotation;
    }

    void Update()
    {
        // update current rotation
        _curRotation = this.transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(_startRotation, _curRotation);

        // falling down tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
            _isCrashed = true;

        // 나무가 쓰러지는 타이밍에 레이어를 한번만 바꿔준다.
        if (_isCrashed && !_isChangedLayer)
            ChangeLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 큰 돌이랑 충돌 시 쓰러짐
        if (collision.gameObject.GetComponent<RollingStone>())
            ExcuteCrash();
    }

    public void ExcuteCrash()
    {
        _rigid.constraints = RigidbodyConstraints2D.None;
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

        parent.transform.GetChild(0).gameObject.layer = ChangeToIndex(_targetLayerMask.value);
        parent.transform.GetChild(1).gameObject.layer = ChangeToIndex(_targetLayerMask.value);

        _isChangedLayer = true;
    }
}
