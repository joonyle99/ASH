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
        Debug.Log("OnCollisionStay ��� ������");

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("�÷��̾� ������ �ִ� ����");

            if (isPushing)
                return;

                // E Ű�� ������������ ȸ�� ���� �ɼ��� �����ϰ�
            if (Input.GetKey(KeyCode.E))
            {
                Debug.Log("E Ű�� ������ �ִ� �����̴�");

                isPushing = true;

                top.GetComponent<Rigidbody2D>().freezeRotation = false;

                // ������ : ������ ������ / ����� : �÷��̾ ������
                float dir = Mathf.Sign(this.transform.position.x - collision.transform.position.x);

                top.GetComponent<Falling_Tree>().FallingTree(dir);
            }
        }
    }
}
