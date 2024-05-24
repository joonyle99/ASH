using UnityEngine;

public class ScaleController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("local scale x ����");

            var localScale = this.transform.localScale;
            localScale.x *= -1f;
            this.transform.localScale = localScale;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("local scale y ����");

            var localScale = this.transform.localScale;
            localScale.y *= -1f;
            this.transform.localScale = localScale;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("local scale z ����");

            var localScale = this.transform.localScale;
            localScale.z *= -1f;
            this.transform.localScale = localScale;
        }
    }
}
