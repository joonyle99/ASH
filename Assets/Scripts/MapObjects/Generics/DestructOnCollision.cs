using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestructEventCaller))]
public class DestructOnCollision : MonoBehaviour
{
    public enum ObjectType
    {
        None, FallingTree, RollingStone, Spikes, StillStone
    }

    [SerializeField] List<ObjectType> _objectList;

    bool IsKillableBy(ObjectType type)
    {
        return _objectList.Find(x => x == type) != ObjectType.None;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //±¼·¯°¡´Â µ¹
        RollingStone stone;
        if ((stone = collision.transform.GetComponent<RollingStone>()) != null)
        {
            if (IsKillableBy(ObjectType.RollingStone) && !stone.IsBreakable)
                Destruction.Destruct(gameObject);
            else if (IsKillableBy(ObjectType.StillStone) && stone.IsBreakable)
                Destruction.Destruct(gameObject);
        }
        //³ª¹«
        else if (IsKillableBy(ObjectType.FallingTree) &&
            collision.transform.GetComponent<FallingTreeTrunk>() != null)
        {
            Destruction.Destruct(gameObject);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {

        //±¼·¯°¡´Â µ¹
        RollingStone stone;
        if ((stone = collision.transform.GetComponent<RollingStone>()) != null)
        {
            if (IsKillableBy(ObjectType.RollingStone) && !stone.IsBreakable)
                Destruction.Destruct(gameObject);
            else if (IsKillableBy(ObjectType.StillStone) && stone.IsBreakable)
                Destruction.Destruct(gameObject);
        }
        //°¡½Ã¹ç
        if (IsKillableBy(ObjectType.Spikes) && collision.transform.GetComponent<Spikes>() != null)
        {
            Destruction.Destruct(gameObject);
        }
    }
}
