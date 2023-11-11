using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCapturer : MonoBehaviour
{
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
        Debug.Log(name + "Hitted by light");
    }
}
