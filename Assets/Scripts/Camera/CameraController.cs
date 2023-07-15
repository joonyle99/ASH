using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.LuisPedroFonseca.ProCamera2D;

public class CameraController : MonoBehaviour, ISceneContextBuildListener
{
    ProCamera2D _proCamera;

    public void OnSceneContextBuilt()
    {
        _proCamera.AddCameraTarget(SceneContextController.Player.transform);
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
