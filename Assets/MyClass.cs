#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class MyClass
{
    static MyClass()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        Debug.Log("Updating");
        UnityEditor.SceneView.RepaintAll();
    }
}
#endif