using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public float value = 1300f;

    void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어 속도가 어느정도 줄어들도록 한다.

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("windArea");

            if (other.gameObject.transform.position.x < this.transform.position.x)
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * value * Time.deltaTime);
            else
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * value * Time.deltaTime);
        }
    }
}
