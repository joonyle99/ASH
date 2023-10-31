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

        // rigidbody�� �������� ���� (�ѹ��� �ϰ������..)
        _rigid.constraints = RigidbodyConstraints2D.None;

        // push tree
        // �������
        // 1. Time.FixedDeltaTime�� ���ؾ� �ϴ°�?
        // 2. Mass�� ���ؾ� �ϴ°�? -> ���ϸ� ������ ����� ���� AddForce()���� ������
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
