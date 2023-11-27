using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    public bool isWorking = false;
    public float value = 15f;
    public GameObject player = null;

    private void Update()
    {
        if (isWorking)
        {
            Debug.Log("isWorking");
        }
    }

    private void FixedUpdate()
    {
        if (isWorking)
        {
            if (player.transform.position.x < this.transform.position.x)
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.left * value, ForceMode2D.Force);
            else
                player.GetComponent<Rigidbody2D>().AddForce(Vector2.right * value, ForceMode2D.Force);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            isWorking = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isWorking = false;
        }
    }
}
