using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class FollowSceneViewCamera : MonoBehaviour
{
    void Update()
    {
        try
        {
            Vector3 sceneViewPos = SceneView.lastActiveSceneView.camera.transform.position;
            sceneViewPos.z = transform.position.z;
            transform.position = sceneViewPos;
        }
        catch { }
    }
}
#endif 
