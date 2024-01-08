using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
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

    void OnEnable()
    {
        this.transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        // Debug.Log(this.transform.lossyScale.x);

        var targets = Physics2D.OverlapCircleAll(transform.position, _radius, _capturerMask);
        if (targets.Length >= 1)
        {
            ShootRays();
        }
    }

    void ShootRays()
    {
        HashSet<Collider2D> hittedColliders = new HashSet<Collider2D>();

        float minAngle = ((lookDir > 0 ? transform.rotation.eulerAngles.z : 180f + transform.rotation.eulerAngles.z) - _angle / 2) * Mathf.Deg2Rad;
        float maxAngle = ((lookDir > 0 ? transform.rotation.eulerAngles.z : 180f + transform.rotation.eulerAngles.z) + _angle / 2) * Mathf.Deg2Rad;

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = Mathf.Lerp(minAngle, maxAngle, (float)i / (_rayCount - 1));
            var hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), _radius, _rayCastLayers);
            if (hit)
            {
                hittedColliders.Add(hit.collider);
            }
        }

        foreach (var col in hittedColliders)
        {
            var capturer = col.GetComponent<LightCapturer>();
            if (capturer != null)
                capturer.OnLightHitted(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        float minAngle = ((lookDir > 0 ? transform.rotation.eulerAngles.z : 180f + transform.rotation.eulerAngles.z) - _angle / 2) * Mathf.Deg2Rad;
        float maxAngle = ((lookDir > 0 ? transform.rotation.eulerAngles.z : 180f + transform.rotation.eulerAngles.z) + _angle / 2) * Mathf.Deg2Rad;

        Gizmos.color = Color.red;

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = Mathf.Lerp(minAngle, maxAngle, (float)i / (_rayCount - 1));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _radius);
        }
    }
}