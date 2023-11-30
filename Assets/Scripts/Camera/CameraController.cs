using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    ProCamera2D _proCamera;
    ProCamera2DShake _shakeComponent;


    public void OnSceneContextBuilt()
    {
        _proCamera.enabled = true;
        _proCamera.AddCameraTarget(SceneContext.Current.Player.transform);
    }
    public void StartFollow(Transform target)
    {
        _proCamera.RemoveAllCameraTargets();
        _proCamera.AddCameraTarget(target);
    }
    public void DisableCameraFollow()
    {
        //_proCamera.enabled = false;
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }
    public void SnapFollow()
    {
        StartCoroutine(SnapFollowCoroutine());
    }
    IEnumerator SnapFollowCoroutine()
    {
        float originalSmoothnessX = _proCamera.HorizontalFollowSmoothness;
        float originalSmoothnessY = _proCamera.VerticalFollowSmoothness;
        _proCamera.HorizontalFollowSmoothness = 0;
        _proCamera.VerticalFollowSmoothness = 0;
        yield return null;
        _proCamera.HorizontalFollowSmoothness = originalSmoothnessX;
        _proCamera.VerticalFollowSmoothness = originalSmoothnessY;

    }
    private void Awake()
    {
        _proCamera = GetComponent<ProCamera2D>();
        _shakeComponent = _proCamera.GetComponent<ProCamera2DShake>();
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void StartShake(ShakePreset preset)
    {
        _shakeComponent.Shake(preset);
    }
    public void StartConstantShake(ConstantShakePreset preset)
    {
        _shakeComponent.StopConstantShaking(0.3f);
        _shakeComponent.ConstantShake(preset);
    }
    public void StopConstantShake(float smooth = 0.1f)
    {
        _shakeComponent.StopConstantShaking(smooth);
    }


}
