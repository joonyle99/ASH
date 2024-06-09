using System.Collections;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    struct InitialSettings
    {
        public Vector2 Offset;
        public Vector2 FollowSmoothness;
    }

    // private InitialSettings _initialSettings;

    private ProCamera2D _proCamera;
    private ProCamera2DShake _shakeComponent;
    private ProCamera2DZoomToFitTargets _fitComponent;

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
        _fitComponent = GetComponent<ProCamera2DZoomToFitTargets>();

        /*
        _initialSettings = new InitialSettings
        {
            Offset = new Vector2(OffsetX, OffsetY),
            FollowSmoothness = new Vector2(_proCamera.HorizontalFollowSmoothness, _proCamera.VerticalFollowSmoothness)
        };
        */
    }

    private void Start()
    {
        // Test
        // TestShake();
    }

    // settings
    public void OnSceneContextBuilt()
    {
        _proCamera.enabled = true;
        if (SceneContext.Current.Player)
            _proCamera.AddCameraTarget(SceneContext.Current.Player.transform);
        SnapFollow();
    }
    public void ResetCameraSettings()
    {
        if (SceneContext.Current.Player)
            StartFollow(SceneContext.Current.Player.transform);

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
    public void AddFollowTargets(Transform [] targets)
    {
        _proCamera.AddCameraTargets(targets);
    }
    public void FollowOnly(Transform target)
    {
        _proCamera.RemoveAllCameraTargets();
        _proCamera.AddCameraTarget(target);
    }
    public void RemoveFollowTargets(Transform[] targets)
    {
        foreach (var target in targets)
        {
            if (target != null)
                _proCamera.RemoveCameraTarget(target);
        }
    }
    public void StartFollow(Transform target, bool removeExisting = true)
    {
        if (removeExisting)
            _proCamera.RemoveAllCameraTargets();            
        _proCamera.AddCameraTarget(target);
    }
    public void RemoveFollowTarget(Transform target)
    {
        _proCamera.RemoveCameraTarget(target);
    }
    public void DisableCameraFollow()
    {
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }
    public void SnapFollow()
    {
        StartCoroutine(SnapFollowCoroutine());
    }
    private IEnumerator SnapFollowCoroutine()
    {
        float originalSmoothnessX = _proCamera.HorizontalFollowSmoothness;
        float originalSmoothnessY = _proCamera.VerticalFollowSmoothness;
        _proCamera.HorizontalFollowSmoothness = 0;
        _proCamera.VerticalFollowSmoothness = 0;
        yield return null;
        _proCamera.HorizontalFollowSmoothness = originalSmoothnessX;
        _proCamera.VerticalFollowSmoothness = originalSmoothnessY;

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

    // TEST FUNCTION
    public void TestShake()
    {
        var defaultShakePreset = _shakeComponent.ShakePresets[0];
        var defaultConstantShakePreset = _shakeComponent.ConstantShakePresets[0];

        // StartShake(defaultShakePreset);
        StartConstantShake(defaultConstantShakePreset);
    }
}
