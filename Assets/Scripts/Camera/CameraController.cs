using System.Collections;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections.Generic;

/// <summary>
/// Main ī�޶� �پ��ִ� ������Ʈ�̸�
/// �� ���ؽ�Ʈ ���� �� OnSceneContextBuilt() �Լ��� ȣ��ȴ�.
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

    private Vector3[] _viewportCorners = new Vector3[4];        // ����Ʈ(ī�޶� ���� ȭ���� '����ȭ'�� 2D ��ǥ �ý���)�� 4�� �ڳ� ��ǥ
    private Vector3[] _worldCorners = new Vector3[4];           // ���� ���������� ����Ʈ �������� �ڳ� ��ǥ
    private Vector3[] _intersectionPoints = new Vector3[4];     // Z == 0�� XY ������ ������

    public Vector3 LeftBottom => _intersectionPoints[0];
    public Vector3 RightBottom => _intersectionPoints[1];
    public Vector3 RightTop => _intersectionPoints[2];
    public Vector3 LeftTop => _intersectionPoints[3];

    // C = A + t * (B - A)
    // A�� �����̰�, t * (B - A)�� A���� �󸶳� �������ִ����� ��Ÿ����.
    // t = 0�̸� A, t = 1�̸� B, 0 < t < 1�̸� A�� B ������ ���
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
            new Vector3(0, 0, _mainCamera.nearClipPlane), // ���ϴ�
            new Vector3(1, 0, _mainCamera.nearClipPlane), // ���ϴ�
            new Vector3(1, 1, _mainCamera.nearClipPlane), // ����
            new Vector3(0, 1, _mainCamera.nearClipPlane)  // �»��

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
            // ����Ʈ ��ǥ(���ϴ�, ���ϴ�, ����, �»��)�� ���� ��ǥ�� ��ȯ
            _worldCorners[i] = _mainCamera.ViewportToWorldPoint(_viewportCorners[i]);

            // ī�޶� ��ġ���� ���� �ڳ� ���������� ���� ����
            Vector3 direction = _worldCorners[i] - _mainCamera.transform.position;
            // direction�� Z == 0�� XY ���� �̷�� ����
            float ratio = (-1) * _mainCamera.transform.position.z / direction.z;
            // direction�� Z == 0�� XY �������� ��� ����
            Vector3 newDirection = new Vector3(direction.x * ratio, direction.y * ratio, (-1) * _mainCamera.transform.position.z);
            // newDirection�� Z == 0�� XY ����� ������
            _intersectionPoints[i] = _mainCamera.transform.position + newDirection;

            // Debug.DrawLine(_mainCamera.transform.position, _intersectionPoints[i], Color.cyan, 0.2f);

            // LeftMiddle, RightMiddle, TopMiddle, BottomMiddle ���� Draw Line length 1
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
        // Debug.Log($"ī�޶� ����");

        if (SceneContext.Current.Player && (SceneContext.Current.CameraType == CameraType.Normal))
        {
            // ī�޶� Ÿ�ٿ� �÷��̸Ӹ� ���Խ�Ų�� (������ Ÿ�� ����)
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

    /// <summary> �ڵ�� �۵� �� ��� </summary>
    public void StartFollow(Transform target, bool removeExisting = true)
    {
        if (removeExisting)
            _proCamera.RemoveAllCameraTargets();

        _proCamera.AddCameraTarget(target);
    }
    /// <summary> CutscenePlayer�� �۵� �� ��� </summary>
    public void FollowOnly(Transform target)
    {
        _proCamera.RemoveAllCameraTargets();
        _proCamera.AddCameraTarget(target);
    }
    public void DisableCameraFollow()
    {
        // 0�� ����� ���� ������ ����
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }

    // effect: snap (�� ������ ���� smoothness�� 0���� ���������ν�, �÷��̾ ��� ���󰡵��� �Ѵ�.)
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
