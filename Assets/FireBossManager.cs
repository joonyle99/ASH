using System.Collections;
using UnityEngine;

public class FireBossManager : MonoBehaviour
{
    [SerializeField] private GameObject _invisibleWall_Left;
    [SerializeField] private GameObject _invisibleWall_Right;

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

    public void SetUpBattle()
    {
        StartCoroutine(SetUpBattleCoroutine());
    }
    private IEnumerator SetUpBattleCoroutine()
    {
        yield return SceneContext.Current.CameraController.ZoomOutCoroutine(-25f);

        Debug.Log("��");

        UseCameraBoundaries();
    }

    public void UseCameraBoundaries()
    {
        // ó���� ���� ������Ʈ�� ��Ȱ��ȭ�Ǿ� ������ Bounds�� ��ȿ���� �ʱ� ������ ���⼭ �����´�
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

    public void UseCameraRooms()
    {

    }
}
