using UnityEngine;
using System.IO;
using UnityEditor.VersionControl;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConverterSpriteToMesh : MonoBehaviour
{
    public string relativePath = "Assets/Project Resources/Meshes";
    public string meshAssetName = "Test";

    [Space]

    public Sprite targetSprite;

#if UNITY_EDITOR
    // Editor���� �����Ҳ��� Ŀ���� �����͸� �����ؼ� make mesh ��ư�� ���� �Ѵ�.
    void Start()
    {
        // ��������Ʈ�� Ÿ�� �̸��� �����Ǿ� �ִ��� Ȯ��
        if (targetSprite == null || string.IsNullOrEmpty(meshAssetName))
        {
            Debug.LogError("Sprite or targetName is not set.");
            return;
        }

        // ��������Ʈ�� �޽÷� ��ȯ
        Mesh mesh = ConvertSpriteToMesh(targetSprite);

        SaveMeshAsAsset(mesh, relativePath);
    }
#endif

    void DisplayFilesInFolder(string path)
    {
        Debug.Log(path);

        // �ش� ����� ��� ������ �����ɴϴ�.
        string[] filePaths = Directory.GetFiles(path);

        // �� ������ ��θ� ����մϴ�.
        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileName(filePath);
            Debug.Log("File: " + fileName);
        }
    }

    Mesh ConvertSpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[sprite.vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = sprite.vertices[i];
        }

        mesh.vertices = vertices;
        mesh.uv = sprite.uv;
        mesh.triangles = System.Array.ConvertAll(sprite.triangles, i => (int)i);

        return mesh;
    }

    void SaveMeshAsAsset(Mesh mesh, string folderPath)
    {
        string fileName = meshAssetName + ".asset";
        string filePath = folderPath + '/' + fileName;

        Debug.Log(filePath);

        AssetDatabase.CreateAsset(mesh, filePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
