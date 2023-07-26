using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    ProCamera2D _proCamera;

    public void OnSceneContextBuilt()
    {
        _proCamera.enabled = true;
        _proCamera.AddCameraTarget(SceneContextController.Player.transform);
    }
    public void DisableCameraFollow()
    {
        //_proCamera.enabled = false;
        _proCamera.HorizontalFollowSmoothness = 100f;
        _proCamera.VerticalFollowSmoothness = 100f;
    }
    private void Awake()
    {
        _proCamera = GetComponent<ProCamera2D>();
    }
    // Update is called once per frame
    void Update()
    {
    }
}
