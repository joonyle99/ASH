using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
class TestDataType
{
    public int x = 0;
    public string y="";
}

public class SaveTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PersistentDataManager.UpdateValueByGlobal<int>("data1", x => x + 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PersistentDataManager.UpdateRefByGlobal<TestDataType>("data2", x => { x.x += 1; x.y += 'a'; });
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PersistentDataManager.TryAddDataGroup("group1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PersistentDataManager.UpdateRef<TestDataType>("group1", "data2", x => { x.x += 1; x.y += 'a'; });
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PersistentDataManager.RemoveDataGroup("group1");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("data1:" + PersistentDataManager.GetByGlobal<int>("data1"));
            Debug.Log("data2:" + PersistentDataManager.GetByGlobal<TestDataType>("data2").y);
            Debug.Log("data2:" + PersistentDataManager.Get<TestDataType>("group1", "data2").y);
        }
    }
}