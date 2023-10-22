using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling_Tree : MonoBehaviour
{
    public GameObject com;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody2D>().centerOfMass = com.transform.position;

        Debug.Log(this.GetComponent<Rigidbody2D>().centerOfMass);
        Debug.Log(com.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FallingTree(float dir)
    {
        Debug.Log("나무가 쓰러진다 ~");

        float degree = dir * 100f;

        if (dir > 0)
            Debug.Log("왼쪽으로 " + degree + "만큼 회전력을 준다 ~");
        else
            Debug.Log("오른쪽으로 " + degree + "만큼 회전력을 준다 ~");

        float radian = Mathf.Deg2Rad * degree;

        this.GetComponent<Rigidbody2D>().AddTorque(radian, ForceMode2D.Impulse);
    }
}
