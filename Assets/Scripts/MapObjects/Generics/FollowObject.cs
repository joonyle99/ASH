using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    public void Update()
    {
        transform.position = target.position + offset;
    }
}
