using NaughtyAttributes.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLantern : Lantern
{
    private void FixedUpdate()
    {
        if(!IsTargetVisible())
        {
            Debug.Log($"Boss lantern out of camera field");
        }
    }

    private bool IsTargetVisible()
    {
        Camera camera = SceneContext.Current.CameraController.GetComponent<Camera>();

        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        var point = transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }

        return true;
    }
}
