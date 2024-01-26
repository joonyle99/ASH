using System.Collections;
using UnityEngine;

public class BearBlocking : MonoBehaviour
{
    public Bear bear;

    private void Start()
    {
        bear.enabled = false;
        bear.Animator.SetTrigger("StartBlocking");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndBlocking();
        }
    }

    public void EndBlocking()
    {
        bear.enabled = true;
        bear.Animator.SetTrigger("EndBlocking");
    }
}
