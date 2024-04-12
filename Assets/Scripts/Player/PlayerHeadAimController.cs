using System;
using UnityEngine;

public class PlayerHeadAimController : MonoBehaviour
{
    [Header("HeadAim")]
    [Space]

    [SerializeField] Transform _target;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _rightMin = 0.9f;
    [SerializeField] float _rightMax = 1.9f;
    [SerializeField] float _leftMin = 0.4f;
    [SerializeField] float _leftMax = 1.4f;
    [SerializeField] float _cameraSpeed = 1f;
    [SerializeField] float _cameraMin = 0.08f;
    [SerializeField] float _cameraMax = 0.68f;

    public Transform Target => _target;
    private PlayerBehaviour _player;

    void Awake()
    {
        _player = GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (_player.CurrentStateIs<IdleState>())
            HeadAimControl();
    }

    private void HeadAimControl()
    {
        // 상태에 따라 자연스러운 카메라 및 targetObject 움직임 구현하기

        // target의 높이 변화에 의한 Head Aim 설정
        // multi aim constraint 사용

        // 왼쪽을 바라볼 때 (recentDir == -1)
        // -> down key -> target object move to up (targetMoveDir == 1)
        // -> up key -> target object move to down (targetMoveDir == -1)
        // 오른쪽을 바라볼 때 (recentDir == 1)
        // -> down key -> target object move to down  (targetMoveDir == -1)
        // -> up key -> target object move to up  (targetMoveDir == 1)

        // 플레이어의 바라보는 방향과 키 입력에 따른 target 오브젝트가 움직이는 방향 설정 (기본값 0)
        float targetObjectMoveDir = 0f;

        if (_player.IsMoveUpKey) targetObjectMoveDir = (_player.RecentDir == 1) ? 1f : -1f;
        else if (_player.IsMoveDownKey) targetObjectMoveDir = (_player.RecentDir == 1) ? -1f : 1f;

        float min = (_player.RecentDir == 1) ? _rightMin : _leftMin;
        float max = (_player.RecentDir == 1) ? _rightMax : _leftMax;

        // target 오브젝트가 위 / 아래로 움직이는 경우
        if (Mathf.Abs(targetObjectMoveDir) > 0.01f)
        {
            // 카메라 이동
            float cameraPosY = SceneContext.Current.Camera.OffsetY;
            float cameraMoveDir = Mathf.Sign(_player.RecentDir * targetObjectMoveDir); // (RecentDir * targetObjectMoveDir) == -1 => down key
            cameraPosY += cameraMoveDir * _cameraSpeed * Time.deltaTime;
            cameraPosY = Mathf.Clamp(cameraPosY, _cameraMin, _cameraMax);
            SceneContext.Current.Camera.OffsetY = cameraPosY;

            // target 오브젝트 이동
            Vector3 targetPos = _target.localPosition;
            targetPos.y += targetObjectMoveDir * _speed * Time.deltaTime;
            targetPos.y = Mathf.Clamp(targetPos.y, min, max);
            _target.localPosition = targetPos;
        }
        // target 오브젝트가 제자리로 돌아가는 경우
        else
        {
            // 카메라 이동
            float cameraPosY = SceneContext.Current.Camera.OffsetY;
            float cameraOriginY = (_cameraMin + _cameraMax) / 2f;
            float cameraMoveDir = cameraOriginY - cameraPosY;
            if (Mathf.Abs(cameraMoveDir) > 0.01f)
            {
                cameraPosY += Mathf.Sign(cameraMoveDir) * _cameraSpeed * Time.deltaTime;
                if (cameraMoveDir < 0f) cameraPosY = Mathf.Max(cameraPosY, cameraOriginY);
                else cameraPosY = Mathf.Min(cameraPosY, cameraOriginY);
                SceneContext.Current.Camera.OffsetY = cameraPosY;
            }

            // targetObject 이동
            Vector3 targetPos = _target.localPosition;
            float targetOriginY = (min + max) / 2f;
            if (Mathf.Abs(targetPos.y - targetOriginY) > 0.01f)
            {
                float taretMoveDir = targetOriginY - targetPos.y;
                targetPos.y += MathF.Sign(taretMoveDir) * _speed / 2f * Time.deltaTime;
                if (targetPos.y > targetOriginY) targetPos.y = Mathf.Clamp(targetPos.y, targetOriginY, max);    // target 오브젝트 이동 방향 : 위 -> 아래
                else targetPos.y = Mathf.Clamp(targetPos.y, min, targetOriginY);                                // target 오브젝트 이동 방향 : 아래 -> 위
                _target.localPosition = targetPos;
            }
        }
    }

    public void HeadAimControlOnFlip()
    {
        // UpdateFlip 할때마다 Target Object의 높이를 맞춰줘야 한다.
        // 0.9 ~ 1.9를 1.4 ~ 0.4에 대응시킨다.
        Vector3 targetVector = _target.localPosition;
        targetVector.y = (_leftMin + _rightMax) - targetVector.y;
        targetVector.y = (_player.RecentDir == 1)
            ? Mathf.Clamp(targetVector.y, _rightMin, _rightMax)
            : Mathf.Clamp(targetVector.y, _leftMin, _leftMax);
        _target.localPosition = targetVector;
    }
}
