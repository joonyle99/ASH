using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float currentTime;
    public float maxTime;

    void Start()
    {
        StartCoroutine(timer());
    }

    public IEnumerator timer()
    {
        currentTime = 0f;
        maxTime = 10f;

        while (!Input.GetKeyDown(KeyCode.Space) || currentTime > maxTime)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log("currentTime");
    }
}
