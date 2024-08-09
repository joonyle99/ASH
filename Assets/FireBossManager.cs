using UnityEngine;

public class FireBossManager : MonoBehaviour
{
    public GameObject InvisibleWall;

    public void TurnOnInvisibleWall()
    {
        InvisibleWall.SetActive(true);
    }
    public void TurnOffInvisibleWall()
    {
        InvisibleWall.SetActive(false);
    }
}
