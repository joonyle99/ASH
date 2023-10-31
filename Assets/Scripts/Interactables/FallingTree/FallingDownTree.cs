using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDownTree : InteractableObject
{
    public Transform forcePointTransform;

    private Rigidbody2D _rigid;

    [SerializeField] private float _power = 50f;
    [SerializeField] private float _mul = 1000f;
    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle = 0f;

    [SerializeField] private bool _isPushed = false;
    [SerializeField] private bool _isFalling = false;

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

    public void FallingDown(float dir)
    {
        // if already falling return
        if (_isFalling)
            return;

        // rigidbody의 제약조건 해제 (한번만 하고싶은데..)
        _rigid.constraints = RigidbodyConstraints2D.None;

        // push tree
        _rigid.AddForceAtPosition(Vector2.right * dir * _power * _mul * Time.deltaTime, forcePointTransform.position, ForceMode2D.Force);
    }

    protected override void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateInteracting()
    {
        throw new System.NotImplementedException();
    }
}
