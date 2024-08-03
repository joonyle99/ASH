using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugBehavior : MonoBehaviour
{
#if UNITY_EDITOR

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            JsonDataManager.Instance.DebugGlobalSaveData();
        }
    }

    [MenuItem("Tools/Find Objects In Layer")]
    public static void FindObjectsInLayer()
    {
        const string targetLayerName = "Player";
        int layerNumber = LayerMask.NameToLayer(targetLayerName);

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layerNumber)
            {
                Debug.Log($"<color=orange><b>Layer</b></color>: {targetLayerName} ========> <color=yellow><b>Object</b></color>: {obj.name}", obj);
            }
        }
    }
#endif
}

[CustomEditor(typeof(DebugBehavior))]
public class DebugBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Find All Preserve State"))
        {
            FindAllPreserveState();
        }
    }

    /// <summary>
    /// 해당 씬에 있는 Preserve State 컴포넌트를 가지는 모든 오브젝트를 출력한다
    /// </summary>
    private void FindAllPreserveState()
    {
        PreserveState[] preserveStates = FindObjectsOfType<PreserveState>();

        foreach (var preserveState in preserveStates)
        {
            Debug.Log($"<color=orange><b>{preserveState.name}</b></color>", preserveState.gameObject);
        }
    }
}