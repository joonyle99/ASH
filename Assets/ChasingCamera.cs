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

        // 시작
        SceneContext.Current.SceneRunningType = SceneRunningType.Chasing;
        SceneContext.Current.CameraController.StartFollow(HelperTrans.transform);
        HelperTrans.position = player.transform.position;

        while (true)
        {
            // 카메라는 타겟 위치를 향해 이동
            var nextHelperPos = new Vector3 (
                player.transform.position.x,
                Mathf.MoveTowards(HelperTrans.position.y, TargetTrans.position.y, Speed * Time.deltaTime),
                player.transform.position.z
                );
            HelperTrans.position = nextHelperPos;

            yield return null;

            // 플레이어가 카메라 안에 있는지 확인
            var playerViewportPos = mainCamera.WorldToViewportPoint(player.HeadTrans.position);
            if (playerViewportPos.x < 0 || playerViewportPos.x > 1 || playerViewportPos.y < 0)
            {
                Debug.Log("카메라 밖으로 플레이어가 나가서 사망합니다.");
                player.CurHp -= player.CurHp;
                break;
            }

            // 카메라가 타겟 위치에 도달했는지 확인
            if (Vector2.Distance(HelperTrans.position, TargetTrans.position) < 3f)
            {
                Debug.Log("카메라가 타겟 위치에 도달했습니다.");
                break;
            }
        }

        // 끝
        SceneContext.Current.SceneRunningType = SceneRunningType.Normal;
        SceneContext.Current.CameraController.StartFollow(player.transform);

        // 다음 연출 실행?
    }
}
