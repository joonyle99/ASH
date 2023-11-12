using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenPath : MonoBehaviour, ILightCaptureListener
{
    [SerializeField] HiddenPathMask _mask;
    [SerializeField] GameObject _coveringWallParent;

    public void OnLightCaptured(LightCapturer capturer, LightSource lightSource)
    {
        _mask.OnLightCaptured();
        Destroy(capturer.gameObject);
        Destroy(_coveringWallParent);
    }
}
