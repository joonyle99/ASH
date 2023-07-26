using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollowSceneView : MonoBehaviour
{

    void Update()
    {
        Debug.Log(SceneView.lastActiveSceneView);
        Vector3 sceneViewPos = SceneView.lastActiveSceneView.camera.transform.position;
        sceneViewPos.z = transform.position.z;
        transform.position = sceneViewPos;
    }
}
