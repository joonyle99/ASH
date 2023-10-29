using UnityEngine;

public class PushState : PlayerState
{
    protected override void OnEnter()
    {
        Debug.Log("Enter PushState");

        this.gameObject.GetComponent<PlayerBehaviour>().Animator.SetBool("IsPush", true);
    }

    protected override void OnUpdate()
    {
        // Debug.Log("Update PushState");
    }

    protected override void OnFixedUpdate()
    {
        // Debug.Log("FixedUpdate PushState");
    }

    protected override void OnExit()
    {
        Debug.Log("Exit PushState");

        this.gameObject.GetComponent<PlayerBehaviour>().Animator.SetBool("IsPush", false);
    }
}