using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOfTree : MonoBehaviour
{
    private Rigidbody2D _rigid;

    [SerializeField] private Transform forcePointTransform;

    [SerializeField] private float _power = 800f;
    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle = 0f;

    [SerializeField] private bool _isPushed = false;
    [SerializeField] private bool _isFalling = false;
    [SerializeField] private float _dir = 0f;

    private Quaternion startRotation;
    private Quaternion curRotation;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        // set start rotation
        startRotation = this.transform.rotation;
    }

    void Update()
    {
        // update current rotation
        curRotation = this.transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(startRotation, curRotation);

        // falling down tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
            _isFalling = true;
    }

    void FixedUpdate()
    {
        if (_isPushed)
        {
            FallDown();
        }
    }

    public void FallDown()
    {
        // if already falling return
        if (_isFalling)
            return;

        // rigidbody의 제약조건 해제 (한번만 하고싶은데..)
        _rigid.constraints = RigidbodyConstraints2D.None;

        // push tree
        // 고려사항
        // 1. Time.FixedDeltaTime을 곱해야 하는가?
        // 2. Mass를 곱해야 하는가? -> 곱하면 질량과 상관이 없는 AddForce()이지 않은가
        _rigid.AddForceAtPosition(Vector2.right * _dir * _power, forcePointTransform.position, ForceMode2D.Force);
    }

    public void ExcutePush(float dir)
    {
        _isPushed = true;
        _dir = dir;
    }

    public void FinishPush()
    {
        _isPushed = false;
        _dir = 0f;
    }
}
