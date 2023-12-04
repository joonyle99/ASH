using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainWorldRotation : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.identity;       
    }
}
