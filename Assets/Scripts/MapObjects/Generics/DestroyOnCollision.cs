using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    public enum ObjectType
    {
        None, FallingTree, RollingStone
    }

    [SerializeField] List<ObjectType> _objectList;

    bool IsKillableBy(ObjectType type)
    {
        return _objectList.Find(x => x == type) != ObjectType.None;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //굴러가는 돌
        if (IsKillableBy(ObjectType.RollingStone) && collision.transform.GetComponent<RollingStone>() != null)
        {
            Destroy(gameObject);
        }
        //나무
        else if (IsKillableBy(ObjectType.FallingTree) &&
            (collision.transform.GetComponent<FallingTreeByCrash>() != null ||
            collision.transform.GetComponent<FallingTreeByPush>() != null))
        {
            Destroy(gameObject);
        }
    }
}
