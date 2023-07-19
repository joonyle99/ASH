using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class CapeController : MonoBehaviour
{
    [SerializeField] Cloth _cape;


    private void Update()
    {
        _cape.externalAcceleration = new Vector3(-Mathf.Sign(transform.lossyScale.x) * Mathf.Abs(_cape.externalAcceleration.x), _cape.externalAcceleration.y, _cape.externalAcceleration.z);
    }

}