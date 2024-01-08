using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    public KeyCode key;
    public float duration;
    public CameraPriority priority = CameraPriority.Lowest;

    IEnumerator CameraEffect()
    {
        CameraControlToken token = new CameraControlToken(priority);
        Debug.Log(Time.time + ": " + gameObject.name + " start effect");
        yield return new WaitUntil(() => token.IsAvailable);
        Debug.Log(Time.time + ": " + gameObject.name + " got token");

        token.Camera?.StartFollow(transform);
        yield return new WaitForSeconds(duration);
        token.Camera?.StartFollow(transform);
        yield return new WaitForSeconds(duration);
        Debug.Log(Time.time + ": " + gameObject.name + " end effect");
        token.Release();
        Debug.Log(Time.time + ": " + gameObject.name + " released token");

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            StartCoroutine(CameraEffect());
        }
    }
}
