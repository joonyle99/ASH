using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    private Vector3 _moveDir;
    private float _speed;

    private bool _isTrigger = false;

    public void SetData(Vector3 newMoveDir, float newSpeed)
    {
        // _moveDir 혹은 _speed 의 기존 설정값이 없는 경우에만 새로운 값으로 설정

        if (_moveDir == Vector3.zero)
        {
            _moveDir = newMoveDir;
        }

        if (Mathf.Approximately(_speed, 0f))
        {
            _speed = newSpeed;
        }
    }
    public void SetTrigger(bool isTrigger)
    {
        _isTrigger = isTrigger;
    }

    private void Update()
    {
        if (_isTrigger == false)
        {
            return;
        }

        // rigidbody(물리)를 사용하지 않는 경우에는 Update에서 이동 처리
        transform.position += _moveDir * _speed * Time.deltaTime;
    }
    //private void FixedUpdate()
    //{
    //    transform.position += _moveDir * _speed;
    //}
}