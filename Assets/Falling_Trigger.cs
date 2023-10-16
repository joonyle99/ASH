using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Trigger : MonoBehaviour
{
    Falling_Tree falling_Tree;

    void Start()
    {
        falling_Tree = this.gameObject.transform.parent.GetComponent<Falling_Tree>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("E 키를 눌러주세요");

        if (other.gameObject.tag == "Player" && !falling_Tree.isFalling)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                falling_Tree.isFalling = true;
            }
        }
    }
}
