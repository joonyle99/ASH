using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<SoundList>().PlayBGM("BGM", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =
            new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

    }

    void FixedUpdate()
    {
        //transform.position =
        //    new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}