using UnityEngine;

public class ChasingCamera : MonoBehaviour
{
    [SerializeField] private Transform _targetTrans;
    [SerializeField] private Transform _helperTrans;

    [Space]

    [SerializeField] private float _speed = 10f;

    private bool _isChasing = false;

    private void LateUpdate()
    {
        if (_isChasing)
        {
            var player = SceneContext.Current.Player;

            // �÷��̾ ī�޶� �ȿ� �ִ��� Ȯ��
            var playerViewportPos = SceneContext.Current.CameraController.MainCamera.WorldToViewportPoint(player.HeadTrans.position);
            if (playerViewportPos.x < 0 || playerViewportPos.x > 1 || playerViewportPos.y < 0)
            {
                Debug.Log("ī�޶� ������ �÷��̾ ������ ����մϴ�.");
                player.CurHp -= player.CurHp;
                RevokeChaseProcess();
                return;
            }

            // ī�޶� Ÿ�� ��ġ�� �����ߴ��� Ȯ��
            if (Vector2.Distance(_helperTrans.position, _targetTrans.position) < 3f)
            {
                Debug.Log("ī�޶� Ÿ�� ��ġ�� �����߽��ϴ�.");
                RevokeChaseProcess();
                return;
            }

            // 2. HelperTrans -> TargetTrans
            var nextHelperPos = new Vector3
                (
                player.transform.position.x,
                Mathf.MoveTowards(_helperTrans.position.y, _targetTrans.position.y, _speed * Time.deltaTime),
                player.transform.position.z
                );
            _helperTrans.position = nextHelperPos;
        }
    }

    public void ExcuteChaseProcess()
    {
        _isChasing = true;

        // 1. Camera -> HelperTrans (Follow)
        SceneContext.Current.SceneRunningType = SceneRunningType.Chasing;
        SceneContext.Current.CameraController.StartFollow(_helperTrans.transform);

        _helperTrans.position = SceneContext.Current.Player.transform.position;
    }
    public void RevokeChaseProcess()
    {
        _isChasing = false;

        // 3. Camera -> Player (Follow) - Reset
        SceneContext.Current.SceneRunningType = SceneRunningType.Normal;
        SceneContext.Current.CameraController.StartFollow(SceneContext.Current.Player.transform);
    }
}
