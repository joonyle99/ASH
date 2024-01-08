using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainWorldRotation : MonoBehaviour
{
    [SerializeField] float zRotation = 0;
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0,0, zRotation));       
    }
}
