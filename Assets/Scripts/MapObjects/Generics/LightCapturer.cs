using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCapturer : MonoBehaviour
{
    [RequireInterface(typeof(ILightCaptureListener))][SerializeField] Object _lightCaptureListenerObject;
    ILightCaptureListener _lightCaptureListener => _lightCaptureListenerObject as ILightCaptureListener;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLightHitted(LightSource lightSource)
    {
        if (_lightCaptureListener != null)
            _lightCaptureListener.OnLightCaptured(this, lightSource);
    }
}
