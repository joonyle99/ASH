using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxAutoScaler : MonoBehaviour
{
    [SerializeField] bool _autoScale = true;
    //Scales object based on distance from camera on z axis, only on editor

    private void Update()
    {
        if (!Selection.Contains(gameObject))
            return;
        if (!_autoScale)
            return;
        float zOffset = transform.position.z - Camera.main.transform.position.z;
        transform.localScale = new Vector3(zOffset, zOffset, 0) / (0 -Camera.main.transform.position.z);
        print(gameObject.name + " " + zOffset.ToString());
    }
    /*
    private void OnDrawGizmosSelected()
    {
        if (!_autoScale)
            return;
        float zOffset = transform.position.z - Camera.main.transform.position.z;
        transform.localScale = new Vector3(zOffset, zOffset, 0) / (-Camera.main.transform.position.z);
        //print(gameObject.name + " " +  zOffset.ToString());
    }*/
}

