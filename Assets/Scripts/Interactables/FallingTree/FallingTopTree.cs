using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 쓰러지는 나무
/// </summary>
public class FallingTopTree : MonoBehaviour
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

    public bool IsFalling { get { return _isFalling; } }

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

        if (!_isPushed && _isFalling)
        {
            // 나무의 레이어를 Ground Layer로 변경해준다.
            ChangeLayer();
        }
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

        // falling tree
        // 힘(N)을 입력하면 강체의 질량과 DT를 고려해서 속도를 변경한다.
        _rigid.AddForceAtPosition(Vector2.right * _dir * _power, forcePointTransform.position, ForceMode2D.Force);
    }

    public void ExcutePush(float dir)
    {
        _isPushed = true;
        _dir = dir;

        // Debug.Log("Excute Push");

        // SceneContext.Current.Player.GetComponent<Animator>().SetBool("IsPush", true);
    }

    public void FinishPush()
    {
        _isPushed = false;
        _dir = 0f;

        // Debug.Log("Finish Push");

        // SceneContext.Current.Player.GetComponent<Animator>().SetBool("IsPush", false);
    }

    private void ChangeLayer()
    {
        string layerName = "Ground";

        // 상위 오브젝트의 레이어 변경
        GameObject parent = this.transform.parent.gameObject;
        parent.layer = LayerMask.NameToLayer(layerName);

        // 그 자식 오브젝트의 레이어 변경
        parent.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(layerName);
        parent.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
