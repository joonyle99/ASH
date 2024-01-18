using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestructEventCaller))]
public class DestructOnCollision : MonoBehaviour
{
    [SerializeField] List<ObjectType> _objectList;

    bool IsKillableBy(ObjectType type)
    {
        return _objectList.Find(x => x == type) != ObjectType.None;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        var otherObject = collision.gameObject.GetComponent<ASHObject>();

        if (otherObject != null && _objectList.Contains(otherObject.Type))
        {
            Destruction.Destruct(gameObject);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var otherObject = collision.gameObject.GetComponent<ASHObject>();
        if (otherObject != null && _objectList.Contains(otherObject.Type))
        {
            Destruction.Destruct(gameObject);
        }
    }
}
