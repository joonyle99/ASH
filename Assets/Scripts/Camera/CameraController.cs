using System.Collections;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections.Generic;

/// <summary>
/// Main 카메라에 붙어있는 컴포넌트이며
/// 씬 컨텍스트 빌드 시 OnSceneContextBuilt() 함수가 호출된다.
/// </summary>
public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    private ProCamera2D _proCamera;
    private ProCamera2DShake _shakeComponent;

    public float OffsetX
    {
        get => _proCamera.OffsetX;
        set => _proCamera.OffsetX = value;
    }
    public float OffsetY
    {
        get => _proCamera.OffsetY;
        set => _proCamera.OffsetY = value;
    }

    private void Awake()
    {
        _proCamera = GetComponent<ProCamera2D>();
        _shakeComponent = GetComponent<ProCamera2DShake>();
    }

    // settings
    public void OnSceneContextBuilt()
    {
        // Debug.Log("call OnSceneContextBuilt");

        _proCamera.enabled = true;

        if (SceneContext.Current.Player)
        {
            _proCamera.RemoveCameraTarget(SceneContext.Current.Player.transform);
            _proCamera.AddCameraTarget(SceneContext.Current.Player.transform);
        }
        else
        {
            Debug.LogWarning("Player not found in the scene");
        }

        SnapFollow();
    }
    public void ResetCameraSettings()
    {
        // Debug.Log($"카메라 리셋");

        if (SceneContext.Current.Player)
        {
            // 카메라 타겟에 플레이머만 포함시킨다 (나머지 타겟 삭제)
            StartFollow(SceneContext.Current.Player.transform);
        }

        // OffsetX = _initialSettings.Offset.x;
        // OffsetY = _initialSettings.Offset.y;

        /*
        Debug.Log(
            $"_proCamera.HorizontalFollowSmoothness: {_proCamera.HorizontalFollowSmoothness}\n" +
            $"_proCamera.VerticalFollowSmoothness: {_proCamera.VerticalFollowSmoothness}\n" +
            $"_initialSettings.FollowSmoothness.x: {_initialSettings.FollowSmoothness.x}\n" +
            $"_initialSettings.FollowSmoothness.y: {_initialSettings.FollowSmoothness.y}");

        _proCamera.HorizontalFollowSmoothness = _initialSettings.FollowSmoothness.x;
        _proCamera.VerticalFollowSmoothness = _initialSettings.FollowSmoothness.y;
        */
    }

    // effect: follow
    public void AddFollowTarget(Transform target)
    {
        _proCamera.AddCameraTarget(target);
    }
    public void AddFollowTargets(Transform[] targets)
    {
        _proCamera.AddCameraTargets(targets);
    }

    public void RemoveFollowTarget(Transform target)
    {
        _proCamera.RemoveCameraTarget(target);
    }
    public void RemoveFollowTargets(Transform[] targets)
    {
        foreach (var target in targets)
        {
            _proCamera.RemoveCameraTarget(target);
        }
    }
    public void RemoveAllFollowTargets()
    {
        _proCamera.RemoveAllCameraTargets();
    }

    public List<CameraTarget> GetAllFollowTargets()
    {
        return _proCamera.CameraTargets;
    }

    /// <summary> 코드로 작동 시 사용 </summary>
    public void StartFollow(Transform target, bool removeExisting = true)
    {
        if (removeExisting)
            _proCamera.RemoveAllCameraTargets();

        _proCamera.AddCameraTarget(target);
    }
    /// <summary> CutscenePlayer로 작동 시 사용 </summary>
    public void FollowOnly(Transform target)
    {
        _proCamera.RemoveAllCameraTargets();
        _proCamera.AddCameraTarget(target);
    }

    public void DisableCameraFollow()
    {
        // 0에 가까울 수록 빠르게 따라감
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }

    // effect: snap (한 프레임 동안 smoothness를 0으로 설정함으로써, 플레이어를 즉시 따라가도록 한다.)
    public void SnapFollow()
    {
        StartCoroutine(SnapFollowCoroutine());
    }
    private IEnumerator SnapFollowCoroutine()
    {
        // Debug.Log("call snap follow coroutine");

        float originalSmoothnessX = _proCamera.HorizontalFollowSmoothness;
        float originalSmoothnessY = _proCamera.VerticalFollowSmoothness;

        // Debug.Log($"_proCamera.HorizontalFollowSmoothness: {_proCamera.HorizontalFollowSmoothness} \n _proCamera.VerticalFollowSmoothness: {_proCamera.VerticalFollowSmoothness}");

        _proCamera.HorizontalFollowSmoothness = 0;
        _proCamera.VerticalFollowSmoothness = 0;

        yield return null;

        _proCamera.HorizontalFollowSmoothness = originalSmoothnessX;
        _proCamera.VerticalFollowSmoothness = originalSmoothnessY;

        // Debug.Log($"_proCamera.HorizontalFollowSmoothness: {_proCamera.HorizontalFollowSmoothness} \n _proCamera.VerticalFollowSmoothness: {_proCamera.VerticalFollowSmoothness}");
    }

    // effect: shake
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
