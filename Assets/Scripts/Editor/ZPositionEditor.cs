using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZPositionEditor : Editor
{
    static void RecursiveCall(Transform transform, int depth)
    {
        // (�ڽ��� �ִ���) ���̻� ���� �ʴ� ����
        if (transform.GetComponent<CameraController>()) return;
        if (transform.GetComponent<ParallaxTool>()) return;
        if (Mathf.Abs(transform.rotation.eulerAngles.x) > 0.000001f) return;
        if (transform.gameObject.CompareTag("Background")) return;

        // �ڽ��� ������ ��������� ȣ��
        foreach (Transform child in transform)
        {
            RecursiveCall(child, depth + 1);
        }

        // zPosition ������ ��ŵ�ϴ� ����
        if (Mathf.Abs(transform.localPosition.z) < 0.000001f) return;

        Debug.Log($"<b><color=yellow>Name</color></b>: {transform.gameObject.name}" +
            $" => " +
            $"<color=orange>Depth</color>: {depth}", transform.gameObject);
    }

    [MenuItem("Tools/Z Position Setter")]
    static void ZPositionChecking()
    {
#if UNITY_EDITOR
        // ���� �ִ� ��� ���� ��Ʈ ������Ʈ�� �����´�.
        var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] gameObjects = currentScene.GetRootGameObjects();

        foreach (var go in gameObjects)
        {
            // Debug.Log($"{go.name}");
            RecursiveCall(go.transform, 0);
        }
#endif
    }
}