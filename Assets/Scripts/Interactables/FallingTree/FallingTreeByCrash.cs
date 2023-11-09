using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ���� ���� �ı��Ǹ� �������� ����
/// </summary>
public class FallingTreeByCrash : MonoBehaviour
{
    private Rigidbody2D _rigid;

    [SerializeField] private float _fallingAngle = 20f;
    [SerializeField] private float _rotatedAngle = 0f;

    [SerializeField] private bool _isCrashed = false;

    private Quaternion startRotation;
    private Quaternion curRotation;

    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();

        // set start rotation
        startRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // update current rotation
        curRotation = this.transform.rotation;

        // calculate rotated angle
        _rotatedAngle = Quaternion.Angle(startRotation, curRotation);

        // falling down tree (you can't push any more)
        if (_rotatedAngle > _fallingAngle)
            _isCrashed = true;

        if (_isCrashed)
        {
            // ������ ���̾ Ground Layer�� �������ش�.
            ChangeLayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ū ���̶� �浹 �� ������
        if (collision.gameObject.GetComponent<RollingStone>())
        {
            Debug.Log("���̶� �浿�ߴ�..!");

            ExcuteCrash();
        }
    }

    public void ExcuteCrash()
    {
        // rigidbody�� �������� ����
        _rigid.constraints = RigidbodyConstraints2D.None;
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
