using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddForce_Velocity_Test : MonoBehaviour
{
    public Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            rigidbody.AddForce(new Vector2(-60f, 60f), ForceMode2D.Impulse);
        else if (Input.GetKeyDown(KeyCode.C))
            rigidbody.AddForce(new Vector2(60f, 60f), ForceMode2D.Impulse);
        else if (Input.GetKeyDown(KeyCode.X))
            rigidbody.velocity = Vector2.zero;
        else if (Input.GetKeyDown(KeyCode.V))
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0;
            rigidbody.velocity = Vector2.left * 10f;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0;
            rigidbody.velocity = Vector2.right * 10f;
        }
    }
}