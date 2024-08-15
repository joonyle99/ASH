using System.Collections;
using UnityEngine;

public class FireBossManager : MonoBehaviour
{
    [SerializeField] private BlazeFire _blazeFire;
    [SerializeField] private Tornado _tornado;

    [Space]

    [SerializeField] private GameObject _invisibleWall_Left;
    [SerializeField] private GameObject _invisibleWall_Right;

    public void PlayerTornadoEffect()
    {
        TurnOffBlazeFire();

        TurnOnTornado();

        _tornado.TornadoAnimation();
    }

    // blaze fire
    public void TurnOnBlazeFire()
    {
        _blazeFire.gameObject.SetActive(true);
    }
    public void TurnOffBlazeFire()
    {
        _blazeFire.gameObject.SetActive(false);
    }

    // tonado
    public void TurnOnTornado()
    {
        _tornado.gameObject.SetActive(true);
    }
    public void TurnOffTornado()
    {
        _tornado.gameObject.SetActive(false);
    }

    // invisible wall
    public void TurnOnInvisibleWall()
    {
        _invisibleWall_Left.SetActive(true);
        _invisibleWall_Right.SetActive(true);
    }
    public void TurnOffInvisibleWall()
    {
        _invisibleWall_Left.SetActive(false);
        _invisibleWall_Right.SetActive(false);
    }

    // boundaries
    public void SetCameraBoundaries()
    {
        // 처음에 게임 오브젝트가 비활성화되어 있으면 Bounds가 유효하지 않기 때문에 여기서 가져온다
        var leftWallCollider = _invisibleWall_Left.GetComponent<BoxCollider2D>();
        var rightWallCollider = _invisibleWall_Right.GetComponent<BoxCollider2D>();

        if (leftWallCollider == null || rightWallCollider == null)
        {
            Debug.LogError("Left or Right Wall Collider is null");
            return;
        }

        var leftValue = _invisibleWall_Left.transform.position.x + leftWallCollider.bounds.extents.x;
        var rightValue = _invisibleWall_Right.transform.position.x - rightWallCollider.bounds.extents.x;

        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Left, true, leftValue);
        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Right, true, rightValue);
    }

    // rooms
    public void SetCameraRooms()
    {

    }

    public void SetUpBattle()
    {
        StartCoroutine(SetUpBattleCoroutine());
    }
    private IEnumerator SetUpBattleCoroutine()
    {
        SceneContext.Current.CameraController.UpdateScreenSize(15f, 2f);

        yield return new WaitUntil(() => SceneContext.Current.CameraController.IsUpdateFinished);

        SetCameraBoundaries();
    }
}
