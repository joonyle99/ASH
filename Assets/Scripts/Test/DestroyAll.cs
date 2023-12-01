using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAll : MonoBehaviour
{
    [SerializeField] GameObject []_objects;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var obj in _objects)
            {
                Destroy(obj);
            }
        }
    }
}
