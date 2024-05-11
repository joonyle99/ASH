using UnityEngine;

public class Fire_AshPillar : MonoBehaviour
{
    public int direction;
    public float speed;

    public void Update()
    {
        transform.position += direction * speed * Vector3.right * Time.deltaTime;
    }

    public void SetDirection(int dir)
    {
        direction = dir;
    }
}
