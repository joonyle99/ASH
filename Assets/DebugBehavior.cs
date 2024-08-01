using UnityEditor;
using UnityEngine;

public class DebugBehavior : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            JsonDataManager.Instance.DebugGlobalSaveData();
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Find Layer Object")]
    public static void FindLayerObject()
    {
        string layerName = "TriggerZoneExceptMonster";
        int layerNumber = LayerMask.NameToLayer(layerName);

        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == layerNumber)
            {
                Debug.Log("Layer: " + layerName + " ------> " + "Object: " + obj.name, obj);
            }
        }
    }
#endif
}
