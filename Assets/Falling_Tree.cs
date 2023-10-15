using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Tree : MonoBehaviour
{
    public GameObject COM;
    public bool isFallingEnd = false;
    public bool isFalling = false;
    public float fallingPower = 30f;
    float time = 0f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            time += Time.deltaTime;

            if (time < 0.5f)
            {
                Fall();
            }
        }
    }

    public void Fall()
    {
        Debug.Log("나무가 쓰러집니다");

        this.GetComponent<Rigidbody2D>().centerOfMass = COM.transform.localPosition;
        this.GetComponent<Rigidbody2D>().AddTorque(-fallingPower, ForceMode2D.Impulse);
    }
}
