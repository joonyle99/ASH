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
        // ���¿� ���� �ڿ������� ī�޶� �� targetObject ������ �����ϱ�

        // target�� ���� ��ȭ�� ���� Head Aim ����
        // multi aim constraint ���

        // ������ �ٶ� �� (recentDir == -1)
        // -> down key -> target object move to up (targetMoveDir == 1)
        // -> up key -> target object move to down (targetMoveDir == -1)
        // �������� �ٶ� �� (recentDir == 1)
        // -> down key -> target object move to down  (targetMoveDir == -1)
        // -> up key -> target object move to up  (targetMoveDir == 1)

        // �÷��̾��� �ٶ󺸴� ����� Ű �Է¿� ���� target ������Ʈ�� �����̴� ���� ���� (�⺻�� 0)
        float targetObjectMoveDir = 0f;

        if (_player.IsMoveUpKey) targetObjectMoveDir = (_player.RecentDir == 1) ? 1f : -1f;
        else if (_player.IsMoveDownKey) targetObjectMoveDir = (_player.RecentDir == 1) ? -1f : 1f;

        float min = (_player.RecentDir == 1) ? _rightMin : _leftMin;
        float max = (_player.RecentDir == 1) ? _rightMax : _leftMax;

        // target ������Ʈ�� �� / �Ʒ��� �����̴� ���
        if (Mathf.Abs(targetObjectMoveDir) > 0.01f)
        {
            // ī�޶� �̵�
            float cameraPosY = SceneContext.Current.Camera.OffsetY;
            float cameraMoveDir = Mathf.Sign(_player.RecentDir * targetObjectMoveDir); // (RecentDir * targetObjectMoveDir) == -1 => down key
            cameraPosY += cameraMoveDir * _cameraSpeed * Time.deltaTime;
            cameraPosY = Mathf.Clamp(cameraPosY, _cameraMin, _cameraMax);
            SceneContext.Current.Camera.OffsetY = cameraPosY;

            // target ������Ʈ �̵�
            Vector3 targetPos = _target.localPosition;
            targetPos.y += targetObjectMoveDir * _speed * Time.deltaTime;
            targetPos.y = Mathf.Clamp(targetPos.y, min, max);
            _target.localPosition = targetPos;
        }
        // target ������Ʈ�� ���ڸ��� ���ư��� ���
        else
        {
            // ī�޶� �̵�
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

            // targetObject �̵�
            Vector3 targetPos = _target.localPosition;
            float targetOriginY = (min + max) / 2f;
            if (Mathf.Abs(targetPos.y - targetOriginY) > 0.01f)
            {
                float taretMoveDir = targetOriginY - targetPos.y;
                targetPos.y += MathF.Sign(taretMoveDir) * _speed / 2f * Time.deltaTime;
                if (targetPos.y > targetOriginY) targetPos.y = Mathf.Clamp(targetPos.y, targetOriginY, max);    // target ������Ʈ �̵� ���� : �� -> �Ʒ�
                else targetPos.y = Mathf.Clamp(targetPos.y, min, targetOriginY);                                // target ������Ʈ �̵� ���� : �Ʒ� -> ��
                _target.localPosition = targetPos;
            }
        }
    }

    public void HeadAimControlOnFlip()
    {
        // UpdateFlip �Ҷ����� Target Object�� ���̸� ������� �Ѵ�.
        // 0.9 ~ 1.9�� 1.4 ~ 0.4�� ������Ų��.
        Vector3 targetVector = _target.localPosition;
        targetVector.y = (_leftMin + _rightMax) - targetVector.y;
        targetVector.y = (_player.RecentDir == 1)
            ? Mathf.Clamp(targetVector.y, _rightMin, _rightMax)
            : Mathf.Clamp(targetVector.y, _leftMin, _leftMax);
        _target.localPosition = targetVector;
    }
}
