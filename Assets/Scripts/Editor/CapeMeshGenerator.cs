using System.Collections;
using System.Linq;
using System.Collections.Generic;
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
[CustomEditor(typeof(CapeMeshGenerator), true)]
public class CapeScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {

        CapeMeshGenerator t = (CapeMeshGenerator)target;
        if(GUILayout.Button("Make mesh"))
        {
            Mesh mesh = t.SpriteToMesh(t.capeSrite);
            AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
            AssetDatabase.SaveAssets();
        }
        DrawDefaultInspector();
        Repaint();
    }
}