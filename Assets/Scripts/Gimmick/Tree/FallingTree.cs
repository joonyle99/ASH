using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTree : InteractableObject
{
    // public GameObject topOfTree;

    public Transform forcePointTransform;
    public GameObject forcePointObj;

    private Rigidbody2D _topRigid;

    [SerializeField] private float _power = 50f;
    [SerializeField] private float _mul = 1000f;
    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle = 0f;

    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Quaternion curRotation;

    [SerializeField] private bool _isPushed = false;
    [SerializeField] private bool _isFalling = false;

    void Start()
    {
        // set rigidbody component
        _topRigid = GetComponent<Rigidbody2D>();

        // draw force point
        Instantiate(forcePointObj, forcePointTransform.position, Quaternion.identity, transform);

        // set start rotation
        startRotation = transform.rotation;
    }

    void Update()
    {
        // update current rotation
        curRotation = transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(startRotation, curRotation);

        // falling tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
        {
            Debug.Log("나무가 쓰러집니다");

            _isFalling = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // check player collision
        if (collision.gameObject.tag == "Player")
        {
            // player collision
            Collision2D player = collision;

            // if already falling return
            if (_isFalling)
                return;

            // interaction key 'E'
            if (Input.GetKey(KeyCode.E))
            {
                // Debug.Log("Push!!!");

                // rigidbody의 제약조건 해제 (freeze rotation ..)
                if (!_isPushed)
                    _topRigid.constraints = RigidbodyConstraints2D.None;

                // set force direction
                float dir = Mathf.Sign(transform.position.x - player.transform.position.x);

                // push tree
                _topRigid.AddForceAtPosition(Vector2.right * dir * _power * _mul * Time.deltaTime, forcePointTransform.position, ForceMode2D.Force);

                _isPushed = true;

                player.gameObject.GetComponent<Animator>().SetBool("IsPush", true);

                // temp
                // if (!player.gameObject.GetComponent<PlayerBehaviour>().StateIs<PushState>())
                    // player.gameObject.GetComponent<PlayerBehaviour>().ChangeState<PushState>();
            }
            else
            {
                player.gameObject.GetComponent<Animator>().SetBool("IsPush", false);
            }
        }
        else
        {
            _isPushed = false;
        }
    }
}
