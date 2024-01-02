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
            SaveDataManager.UpdateValue<int>("data1", x => x + 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SaveDataManager.UpdateRef<TestDataType>("data2", x => { x.x += 1; x.y += 'a'; }); 
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("data1:" + SaveDataManager.Get<int>("data1"));
            Debug.Log("data2:" + SaveDataManager.Get<TestDataType>("data2").y);
        }
    }
}