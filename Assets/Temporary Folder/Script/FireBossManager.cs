using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static SceneTransitionPlayer;

public class FireBossManager : MonoBehaviour
{
    [SerializeField] private BlazeFire _blazeFire;
    [SerializeField] private Tornado _tornado;

    [Space]

    [SerializeField] private GameObject _invisibleWall_Left;
    [SerializeField] private GameObject _invisibleWall_Right;

    [Space]

    [SerializeField] private ParticleHelper _rageAshEffectEmitting;

    [Space]

    [SerializeField] private GameObject _footholds;

    // effects
    public void ExcuteTornadoEffect()
    {
        _blazeFire.gameObject.SetActive(false);
        _tornado.gameObject.SetActive(true);

        _tornado.TornadoAnimation();
    }

    // ...
    public void ExcuteRageEffect()
    {
        StartCoroutine(ExcuteRageEffectCoroutine());
    }
    private IEnumerator ExcuteRageEffectCoroutine()
    {
        // playing
        var effectObject = _rageAshEffectEmitting.gameObject;
        effectObject.SetActive(true);

        // emitting
        var particle = _rageAshEffectEmitting.ParticleSystem;
        var burst = particle.emission.GetBurst(0);
        var burstDuration = (burst.cycleCount - 1) * burst.repeatInterval;
        yield return new WaitForSeconds(burstDuration);
        yield return new WaitForSeconds(0.2f);

        // pause
        particle.Pause();
        yield return new WaitForSeconds(5f);

        // fade out
        yield return SceneContext.Current.SceneTransitionPlayer.FadeCoroutine(2f, FadeType.Darken);

        effectObject.SetActive(false);
        yield return new WaitForSeconds(3f);

        // fade in
        yield return SceneContext.Current.SceneTransitionPlayer.FadeCoroutine(2f, FadeType.Dim);
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
        // set boundaries
        _invisibleWall_Left.SetActive(true);
        _invisibleWall_Right.SetActive(true);
        SetCameraBoundaries();

        // set camera size
        SceneContext.Current.CameraController.UpdateScreenSize(13f, 2f);

        yield return new WaitUntil(() => SceneContext.Current.CameraController.IsUpdateFinished);

        yield return null;
    }
}
