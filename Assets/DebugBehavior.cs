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
}
