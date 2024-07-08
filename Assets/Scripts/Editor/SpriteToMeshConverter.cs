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
    // Editor에서 제작할꺼면 커스텀 에디터를 생성해서 make mesh 버튼을 만들어서 한다.
    void Start()
    {
        // 스프라이트와 타겟 이름이 설정되어 있는지 확인
        if (targetSprite == null || string.IsNullOrEmpty(meshAssetName))
        {
            Debug.LogError("Sprite or targetName is not set.");
            return;
        }

        // 스프라이트를 메시로 변환
        Mesh mesh = ConvertSpriteToMesh(targetSprite);

        SaveMeshAsAsset(mesh, relativePath);
    }
#endif

    void DisplayFilesInFolder(string path)
    {
        Debug.Log(path);

        // 해당 경로의 모든 파일을 가져옵니다.
        string[] filePaths = Directory.GetFiles(path);

        // 각 파일의 경로를 출력합니다.
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
