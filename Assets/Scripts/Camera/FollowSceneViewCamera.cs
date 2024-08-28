#if UNITY_EDITOR
using UnityEditor;
#endif 

using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class FollowSceneViewCamera : MonoBehaviour
{
    void Update()
    {
        if (Application.isPlaying)
            return;
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