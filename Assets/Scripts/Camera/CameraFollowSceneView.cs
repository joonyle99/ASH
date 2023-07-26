using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif 
public class CameraFollowSceneView : MonoBehaviour
{

#if UNITY_EDITOR
    void Update()
    {
        Vector3 sceneViewPos = SceneView.lastActiveSceneView.camera.transform.position;
        sceneViewPos.z = transform.position.z;
        transform.position = sceneViewPos;
    }
#endif 
}
