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

// CapeMeshGenerator 스크립트에 대한 커스텀 에디터를 정의합니다.
// true는 커스텀 에디터가 CapeMeshGenerator 클래스의 '서브클래스'에서도 작동하도록 지정합니다.
[CustomEditor(typeof(CapeMeshGenerator), true)]

// Editor 클래스를 상속받아 Unity 에디터용 '커스텀 인스펙터'를 생성합니다.
public class CapeScriptEditor : Editor
{
    // Unity 에디터에서 인스펙터 GUI를 그리는 메서드입니다.
    // OnInspectorGUI 메서드를 오버라이드하여 커스텀 GUI를 정의합니다.
    public override void OnInspectorGUI()
    {
        // 현재 인스펙터에서 편집 중인 CapeMeshGenerator '객체'를 가져옵니다.
        CapeMeshGenerator t = (CapeMeshGenerator)target;

        // "Make mesh"라는 레이블이 있는 버튼을 만듭니다.
        // 버튼을 클릭하면 내부 코드를 실행합니다.
        if (GUILayout.Button("Make mesh"))
        {
            // SpriteToMesh 메서드를 호출하여 스프라이트를 메시로 변환합니다.
            Mesh mesh = t.SpriteToMesh(t.capeSrite);
            // 변환된 메쉬를 "Assets/mesh.asset" 경로에 생성하고 저장합니다.
            AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
            AssetDatabase.SaveAssets();
        }
        // 기본 인스펙터 GUI를 그립니다.
        DrawDefaultInspector();
        // 인스펙터를 다시 그리도록 합니다.
        Repaint();
    }
}