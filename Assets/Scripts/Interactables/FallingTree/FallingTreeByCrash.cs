using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 굴러오는 돌에 의해 파괴되며 쓰러지는 나무
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
            // 나무의 레이어를 Ground Layer로 변경해준다.
            ChangeLayer();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 큰 돌이랑 충돌 시 쓰러짐
        if (collision.gameObject.GetComponent<RollingStone>())
        {
            Debug.Log("돌이랑 충동했다..!");

            ExcuteCrash();
        }
    }

    public void ExcuteCrash()
    {
        // rigidbody의 제약조건 해제
        _rigid.constraints = RigidbodyConstraints2D.None;
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
