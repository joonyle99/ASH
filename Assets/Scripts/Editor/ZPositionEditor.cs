using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZPositionEditor : Editor
{
    static void RecursiveCall(Transform transform, int depth)
    {
        // (자식이 있더라도) 더이상 들어가지 않는 조건
        if (transform.GetComponent<CameraController>()) return;
        if (transform.GetComponent<ParallaxTool>()) return;

        // 자식이 있으면 재귀적으로 호출
        foreach (Transform child in transform)
        {
            RecursiveCall(child, depth + 1);
        }

        // zPosition 세팅을 스킵하는 조건
        if (Mathf.Abs(transform.localPosition.z) < 0.000001f) return;
        var renderer = transform.GetComponent<Renderer>();
        if (renderer)
        {
            var sortinglayerName = renderer.sortingLayerName;
            if (sortinglayerName.Equals("Background1") ||
                sortinglayerName.Equals("Background2") ||
                sortinglayerName.Equals("Background3") ||
                sortinglayerName.Equals("Background4") ||
                sortinglayerName.Equals("Background5") ||
                sortinglayerName.Equals("Foreground1") ||
                sortinglayerName.Equals("Foreground2"))
            {
                return;
            }
        }

        Debug.Log($"<b><color=yellow>Name</color></b>: {transform.gameObject.name}" +
            $"=>" +
            $"<color=orange>Depth</color>: {depth}", transform.gameObject);
    }

    static void RecursiveTest(Transform transform, int depth)
    {
        if (transform.childCount == 0) return;

        foreach (Transform child in transform)
        {
            RecursiveTest(child, depth + 1);
        }

        Debug.Log($"depth: {depth} // name: {transform.gameObject.name}", transform.gameObject);
    }

    [MenuItem("Tools/Z Position Setter")]
    static void ZPositionChecking()
    {
#if UNITY_EDITOR
        // 씬에 있는 모든 게임 루트 오브젝트를 가져온다.
        var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] gameObjects = currentScene.GetRootGameObjects();

        RecursiveCall(gameObjects[0].transform, 0);
#endif
    }
}