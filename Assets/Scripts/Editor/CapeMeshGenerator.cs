using System.Linq;
using UnityEngine;
using UnityEditor;

public class CapeMeshGenerator : MonoBehaviour
{
    public Sprite capeSrite;

    public Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(System.Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(System.Array.ConvertAll(sprite.triangles, i => (int)i), 0);

        return mesh;
    }
}

// CapeMeshGenerator ��ũ��Ʈ�� ���� Ŀ���� �����͸� �����մϴ�.
// true�� Ŀ���� �����Ͱ� CapeMeshGenerator Ŭ������ '����Ŭ����'������ �۵��ϵ��� �����մϴ�.
[CustomEditor(typeof(CapeMeshGenerator), true)]

// Editor Ŭ������ ��ӹ޾� Unity �����Ϳ� 'Ŀ���� �ν�����'�� �����մϴ�.
public class CapeScriptEditor : Editor
{
    // Unity �����Ϳ��� �ν����� GUI�� �׸��� �޼����Դϴ�.
    // OnInspectorGUI �޼��带 �������̵��Ͽ� Ŀ���� GUI�� �����մϴ�.
    public override void OnInspectorGUI()
    {
        // ���� �ν����Ϳ��� ���� ���� CapeMeshGenerator '��ü'�� �����ɴϴ�.
        CapeMeshGenerator t = (CapeMeshGenerator)target;

        // "Make mesh"��� ���̺��� �ִ� ��ư�� ����ϴ�.
        // ��ư�� Ŭ���ϸ� ���� �ڵ带 �����մϴ�.
        if (GUILayout.Button("Make mesh"))
        {
            // SpriteToMesh �޼��带 ȣ���Ͽ� ��������Ʈ�� �޽÷� ��ȯ�մϴ�.
            Mesh mesh = t.SpriteToMesh(t.capeSrite);
            // ��ȯ�� �޽��� "Assets/mesh.asset" ��ο� �����ϰ� �����մϴ�.
            AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
            AssetDatabase.SaveAssets();
        }
        // �⺻ �ν����� GUI�� �׸��ϴ�.
        DrawDefaultInspector();
        // �ν����͸� �ٽ� �׸����� �մϴ�.
        Repaint();
    }
}