using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : MonoBehaviour
{
    float time;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * transform.localScale.z * 10 * Time.deltaTime;

        time += Time.deltaTime;

        if(time > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            float dir = Mathf.Sign(collision.transform.position.x - transform.position.x);
            Vector2 vec = new Vector2(10f * dir, 10f / 2f);

            // gameObject.SetActive(false);
            collision.GetComponent<BasedMonster>().OnDamage(30);
            collision.GetComponent<BasedMonster>().KnockBack(vec);

            Destroy(gameObject);
        }
    }
}
