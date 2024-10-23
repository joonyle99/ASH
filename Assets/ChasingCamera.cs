using System.Collections;
using UnityEngine;

public class ChasingCamera : MonoBehaviour
{
    public Transform TargetTrans;
    public Transform HelperTrans;

    [Space]

    public float Speed = 10f;

    public void ExcuteChaseProcess()
    {
        StartCoroutine(ChaseProcessCoroutine());
    }
    private IEnumerator ChaseProcessCoroutine()
    {
        var player = SceneContext.Current.Player;
        var mainCamera = SceneContext.Current.CameraController.MainCamera;

        // ����
        SceneContext.Current.SceneRunningType = SceneRunningType.Chasing;
        SceneContext.Current.CameraController.StartFollow(HelperTrans.transform);
        HelperTrans.position = player.transform.position;

        while (true)
        {
            // ī�޶�� Ÿ�� ��ġ�� ���� �̵�
            var nextHelperPos = new Vector3 (
                player.transform.position.x,
                Mathf.MoveTowards(HelperTrans.position.y, TargetTrans.position.y, Speed * Time.deltaTime),
                player.transform.position.z
                );
            HelperTrans.position = nextHelperPos;

            yield return null;

            // �÷��̾ ī�޶� �ȿ� �ִ��� Ȯ��
            var playerViewportPos = mainCamera.WorldToViewportPoint(player.HeadTrans.position);
            if (playerViewportPos.x < 0 || playerViewportPos.x > 1 || playerViewportPos.y < 0)
            {
                Debug.Log("ī�޶� ������ �÷��̾ ������ ����մϴ�.");
                player.CurHp -= player.CurHp;
                break;
            }

            // ī�޶� Ÿ�� ��ġ�� �����ߴ��� Ȯ��
            if (Vector2.Distance(HelperTrans.position, TargetTrans.position) < 3f)
            {
                Debug.Log("ī�޶� Ÿ�� ��ġ�� �����߽��ϴ�.");
                break;
            }
        }

        // ��
        SceneContext.Current.SceneRunningType = SceneRunningType.Normal;
        SceneContext.Current.CameraController.StartFollow(player.transform);

        // ���� ���� ����?
    }
}
