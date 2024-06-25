using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ZPositionEditor : Editor
{
    static void RecursiveCall(Transform transform, int depth)
    {
        // �ڽ��� ���̻� ���� ��� ����
        if (transform.childCount == 0) return;

        // (�ڽ��� �ִ���) ���̻� ���� �ʴ� ����
        if (transform.GetComponent<CameraController>()) return;
        if (transform.GetComponent<ParallaxTool>()) return;

        // �ڽ��� ������ ��������� ȣ��
        foreach (Transform child in transform)
        {
            RecursiveCall(child, depth + 1);
        }

        // zPosition ������ ��ŵ�ϴ� ����
        if (Mathf.Abs(transform.localPosition.z) < 0.01f) return;
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

        Debug.Log(transform.gameObject.name, transform.gameObject);
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
    static void PrintZPosition()
    {
#if UNITY_EDITOR
        // ���� �ִ� ��� ���� ��Ʈ ������Ʈ�� �����´�.
        var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        GameObject[] gameObjects = currentScene.GetRootGameObjects();

        // ���� ������Ʈ�� zPosition�� 0�� �ƴ� ��ü���� ����� ǥ���Ѵ�
        foreach (var gameObject in gameObjects)
        {
            RecursiveCall(gameObject.transform, 0);
        }
#endif
    }
}