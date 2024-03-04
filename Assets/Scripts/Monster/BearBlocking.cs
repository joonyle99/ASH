using System.Collections;
using UnityEngine;

public class BearBlocking : MonoBehaviour
{
    public Bear bear;

    private void Start()
    {
        StartBlocking();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            EndBlocking();
        }
    }

    public void StartBlocking()
    {
        bear.enabled = false;
        bear.Animator.SetTrigger("StartBlocking");
    }

    public void EndBlocking()
    {
        bear.enabled = true;
        bear.Animator.SetTrigger("EndBlocking");
    }

    public void EndBlocking_OnlyEnable()
    {
        bear.enabled = true;
    }
}
