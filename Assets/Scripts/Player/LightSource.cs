using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LightSource : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] float _angle;
    [Range(2, 360)][SerializeField] int _rayCount;

    [SerializeField] LayerMask _capturerMask;
    [SerializeField] LayerMask _rayCastLayers;

    int lookDir => Math.Sign(transform.lossyScale.x);
    // Update is called once per frame
    void Update()
    {
        var targets = Physics2D.OverlapCircleAll(transform.position, _radius, _capturerMask);
        if (targets.Length >= 1)
        {
            ShootRays();
        }
    }


    void ShootRays()
    {
        HashSet<Transform> hittedTransforms = new HashSet<Transform>();
        float minAngle = (transform.rotation.eulerAngles.z - _angle / 2) * Mathf.Deg2Rad;
        float maxAngle = (transform.rotation.eulerAngles.z + _angle / 2) * Mathf.Deg2Rad;
        for(int i=0; i<_rayCount; i++)
        {
            float angle = Mathf.Lerp(minAngle, maxAngle, (float)i / (_rayCount-1));
            var hit = Physics2D.Raycast(transform.position, new Vector2(lookDir * Mathf.Cos(angle), Mathf.Sin(angle)), _radius, _rayCastLayers);
            if(hit)
            {
                hittedTransforms.Add(hit.transform);
            }
        }
        foreach(Transform t in hittedTransforms)
        {
            var capturer = t.GetComponent<LightCapturer>();
            if (capturer != null)
                capturer.OnLightHitted(this);
        }
    }
    private void OnDrawGizmosSelected()
    {
        float minAngle = (transform.rotation.eulerAngles.z - _angle / 2) * Mathf.Deg2Rad;
        float maxAngle = (transform.rotation.eulerAngles.z + _angle / 2) * Mathf.Deg2Rad;
        Gizmos.color = Color.red;
        for (int i = 0; i < _rayCount; i++)
        {
            float angle = Mathf.Lerp(minAngle, maxAngle, (float)i / (_rayCount - 1));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(lookDir * Mathf.Cos(angle), Mathf.Sin(angle), 0) * _radius);

        }
    }
}
