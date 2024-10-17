#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class EditorUpdater
{
    private static bool _useUpdater = false;

    static EditorUpdater()
    {
        EditorApplication.update -= UpdateFunction;
        EditorApplication.update += UpdateFunction;
    }

    static void UpdateFunction()
    {
        if (_useUpdater == false) return;

        Debug.Log("Updating");
        UnityEditor.SceneView.RepaintAll();
    }
}
#endif