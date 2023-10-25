using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Tree : MonoBehaviour
{
    private Rigidbody2D _rigid;

    public Transform forcePointTransform;
    public GameObject forcePointObj;

    public float power = 50f;
    public float mul = 1000f;

    private bool _isPushing = false;
    private bool _isFalling = false;
    private float _elapsedTime = 0f;
    private float _durationTime = 2f;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        Instantiate(forcePointObj, forcePointTransform.position, Quaternion.identity, this.transform);
    }

    void Update()
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Debug.Log("OnCollisionStay 계속 도는중");

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 접근해 있는 상태");

            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("Push!!!");

                if (!_isPushing && !_isFalling)
                    _rigid.constraints = RigidbodyConstraints2D.None;

                _rigid.AddForceAtPosition(Vector2.right * power * mul * Time.deltaTime, forcePointTransform.position, ForceMode2D.Force);
                _isPushing = true;
            }
        }
    }
}
