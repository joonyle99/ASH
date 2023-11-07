using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ����
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
            // ������ ���̾ Ground Layer�� �������ش�.
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

        // rigidbody�� �������� ���� (�ѹ��� �ϰ������..)
        _rigid.constraints = RigidbodyConstraints2D.None;

        // falling tree
        // ��(N)�� �Է��ϸ� ��ü�� ������ DT�� ����ؼ� �ӵ��� �����Ѵ�.
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

        // ���� ������Ʈ�� ���̾� ����
        GameObject parent = this.transform.parent.gameObject;
        parent.layer = LayerMask.NameToLayer(layerName);

        // �� �ڽ� ������Ʈ�� ���̾� ����
        parent.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(layerName);
        parent.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
