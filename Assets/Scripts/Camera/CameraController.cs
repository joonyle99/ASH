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
    public enum BoundaryType
    {
        Top,
        Bottom,
        Left,
        Right
    }

    private Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    private Vector3[] _viewportCorners = new Vector3[4];        // 뷰포트(카메라가 보는 화면의 '정규화'된 2D 좌표 시스템)의 4개 코너 좌표
    private Vector3[] _worldCorners = new Vector3[4];           // 월드 공간에서의 뷰포트 프러스텀 코너 좌표
    private Vector3[] _intersectionPoints = new Vector3[4];     // Z == 0인 XY 평면과의 교차점

    public Vector3 LeftBottom => _intersectionPoints[0];
    public Vector3 RightBottom => _intersectionPoints[1];
    public Vector3 RightTop => _intersectionPoints[2];
    public Vector3 LeftTop => _intersectionPoints[3];

    // C = A + t * (B - A)
    // A가 기준이고, t * (B - A)는 A에서 얼마나 떨어져있는지를 나타낸다.
    // t = 0이면 A, t = 1이면 B, 0 < t < 1이면 A와 B 사이의 어딘가
    public Vector3 LeftMiddle => (LeftBottom + LeftTop) / 2f;       // = LeftBottom + 0.5f * (LeftTop - LeftBottom) = (LeftBottom + LeftTop) / 2f
    public Vector3 RightMiddle => (RightBottom + RightTop) / 2f;
    public Vector3 TopMiddle => (RightTop + LeftTop) / 2f;
    public Vector3 BottomMiddle => (LeftBottom + RightBottom) / 2f;

    private ProCamera2D _proCamera;
    public ProCamera2D ProCamera => _proCamera;

    private ProCamera2DShake _shakeComponent;
    private ProCamera2DTriggerZoom _triggerZoomComponent;
    private ProCamera2DZoomToFitTargets _zoomToFitTargetsComponent;
    private ProCamera2DNumericBoundaries _boundariesComponent;
    private ProCamera2DRooms _roomsComponent;

    /*
    public ShakePreset largeexplosion;
    public ConstantShakePreset beamshoot;
    */

    public bool IsUpdateFinished { get; private set; }

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
        _mainCamera = GetComponent<Camera>();
        _proCamera = GetComponent<ProCamera2D>();

        _shakeComponent = GetComponent<ProCamera2DShake>();
        _triggerZoomComponent = GetComponent<ProCamera2DTriggerZoom>();
        _zoomToFitTargetsComponent = GetComponent<ProCamera2DZoomToFitTargets>();
        _boundariesComponent = GetComponent<ProCamera2DNumericBoundaries>();
        _roomsComponent = GetComponent<ProCamera2DRooms>();

        _viewportCorners = new Vector3[]
        {
            new Vector3(0, 0, _mainCamera.nearClipPlane), // 좌하단
            new Vector3(1, 0, _mainCamera.nearClipPlane), // 우하단
            new Vector3(1, 1, _mainCamera.nearClipPlane), // 우상단
            new Vector3(0, 1, _mainCamera.nearClipPlane)  // 좌상단

            // (0,1)-------------(1,1)
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            //   |                 |
            // (0,0)-------------(1,0)
        };

        _proCamera.OnUpdateScreenSizeFinished -= OnUpdateFinished;
        _proCamera.OnUpdateScreenSizeFinished += OnUpdateFinished;
    }
    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            // largeexplosion
            StartShake(largeexplosion);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // beamshoot
            StartConstantShake(beamshoot);
        }
        */
    }
    private void LateUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            // 뷰포트 좌표(좌하단, 우하단, 우상단, 좌상단)를 월드 좌표로 변환
            _worldCorners[i] = _mainCamera.ViewportToWorldPoint(_viewportCorners[i]);

            // 카메라 위치에서 월드 코너 지점으로의 방향 벡터
            Vector3 direction = _worldCorners[i] - _mainCamera.transform.position;
            // direction이 Z == 0인 XY 평면과 이루는 비율
            float ratio = (-1) * _mainCamera.transform.position.z / direction.z;
            // direction을 Z == 0인 XY 평면까지의 쏘는 벡터
            Vector3 newDirection = new Vector3(direction.x * ratio, direction.y * ratio, (-1) * _mainCamera.transform.position.z);
            // newDirection과 Z == 0인 XY 평면의 교차점
            _intersectionPoints[i] = _mainCamera.transform.position + newDirection;

            // Debug.DrawLine(_mainCamera.transform.position, _intersectionPoints[i], Color.cyan, 0.2f);

            // LeftMiddle, RightMiddle, TopMiddle, BottomMiddle 에서 Draw Line length 1
            Debug.DrawLine(LeftMiddle, LeftMiddle + Vector3.forward, Color.red);
            Debug.DrawLine(RightMiddle, RightMiddle + Vector3.forward, Color.green);
            Debug.DrawLine(TopMiddle, TopMiddle + Vector3.forward, Color.blue);
            Debug.DrawLine(BottomMiddle, BottomMiddle + Vector3.forward, Color.yellow);
        }
    }
    private void OnDestroy()
    {
        _proCamera.OnUpdateScreenSizeFinished -= OnUpdateFinished;
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

        if (SceneContext.Current.Player && (SceneContext.Current.CameraType == CameraType.Normal))
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
    public void EnableFollow()
    {
        _proCamera.FollowHorizontal = true;
        _proCamera.FollowVertical = true;
    }
    public void DisableFollow()
    {
        _proCamera.FollowHorizontal = false;
        _proCamera.FollowVertical = false;
    }

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
    public IEnumerator SnapFollowCoroutine()
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
        // _shakeComponent.StopConstantShaking(0.3f);
        _shakeComponent.ConstantShake(preset);
    }
    public void StopConstantShake(float smooth = 0.1f)
    {
        _shakeComponent.StopConstantShaking(smooth);
    }

    // effect: boundaries
    public void SetBoundaries(BoundaryType type, bool isOn, float value)
    {
        if (!_boundariesComponent.UseNumericBoundaries)
            _boundariesComponent.UseNumericBoundaries = isOn;

        switch (type)
        {
            case BoundaryType.Top:
                _boundariesComponent.UseTopBoundary = isOn;
                _boundariesComponent.TopBoundary = value;
                break;
            case BoundaryType.Bottom:
                _boundariesComponent.UseBottomBoundary = isOn;
                _boundariesComponent.BottomBoundary = value;
                break;
            case BoundaryType.Left:
                _boundariesComponent.UseLeftBoundary = isOn;
                _boundariesComponent.LeftBoundary = value;
                break;
            case BoundaryType.Right:
                _boundariesComponent.UseRightBoundary = isOn;
                _boundariesComponent.RightBoundary = value;
                break;
        }
    }

    // effect: zoom
    public void UpdateScreenSize(float target, float duration = 2f)
    {
        IsUpdateFinished = false;
        _proCamera.UpdateScreenSize(target, duration);
    }
    public void OnUpdateFinished(float t)
    {
        // Debug.Log("OnUpdateFinished");
        IsUpdateFinished = true;
    }

    /*
    public void ZoomOut(float target)
    {
        StartCoroutine(ZoomOutCoroutine(target));
    }
    public IEnumerator ZoomOutCoroutine(float target)
    {
        var start = _proCamera.transform.position.z;

        var eTime = 0f;
        while (eTime < 1.5f)
        {
            var t = joonyle99.Math.EaseOutQuad(eTime / 1.5f);
            var next = Mathf.Lerp(start, target, t);

            _proCamera.transform.position =
                new Vector3(_proCamera.transform.position.x, _proCamera.transform.position.y, next);

            eTime += Time.deltaTime;
            yield return null;
        }

        _proCamera.transform.position = new Vector3(_proCamera.transform.position.x, _proCamera.transform.position.y, target);
    }
    */

    // effect: zoom to fit targets
    public void TurnOnZoomToFitTargets()
    {
        _zoomToFitTargetsComponent.enabled = true;
    }
    public void TurnOffZoomToFitTargets()
    {
        _zoomToFitTargetsComponent.enabled = false;
    }
}
