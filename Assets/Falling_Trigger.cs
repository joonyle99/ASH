using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Trigger : MonoBehaviour
{
    public GameObject top;
    public bool isPushing;

    void Start()
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("OnCollisionStay 계속 도는중");

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 접근해 있는 상태");

            if (isPushing)
                return;

                // E 키를 누르고있으면 회전 잠을 옵션을 해제하고
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("E 키를 누르고 있는 상태이다");

                isPushing = true;

                top.GetComponent<Rigidbody2D>().freezeRotation = false;

                // 음수값 : 나무가 오른쪽 / 양수값 : 플레이어가 오른쪽
                float dir = Mathf.Sign(this.transform.position.x - collision.transform.position.x);

                top.GetComponent<Falling_Tree>().FallingTree(dir);
            }
        }
    }
}
