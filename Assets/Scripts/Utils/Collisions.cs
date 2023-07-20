using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Collisions
    {
        public static T GetCollidedComponent<T>(Collider2D collider) where T : MonoBehaviour
        {
            ContactFilter2D filter = new ContactFilter2D();
            List<Collider2D> results = new List<Collider2D>();
            collider.OverlapCollider(filter.NoFilter(), results);

            T result = null;
            foreach (Collider2D c in results)
            {
                result = c.GetComponent<T>();
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}