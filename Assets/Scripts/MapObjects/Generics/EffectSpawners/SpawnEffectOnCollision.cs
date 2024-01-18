using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnEffectOnCollision : MonoBehaviour
{
    [System.Serializable]
    public struct CollisionCondition
    {
        public bool PlayOnce;
        public bool RotateToNormal;
        public float Velocity;
        public ObjectType ObjectType;
        public ParticleHelper EffectPrefab;
    }
    [SerializeField] bool _logVelocity = false;
    [SerializeField] List<CollisionCondition> _conditions;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherObject = collision.gameObject.GetComponent<ASHObject>();

        if (otherObject != null)
        {
            if (_logVelocity)
                Debug.Log("Collision with " + otherObject.Type + ": " + collision.relativeVelocity.magnitude);
            var matches = _conditions.FindAll(x => x.ObjectType == otherObject.Type
                                                  && ((x.Velocity * x.Velocity) <= collision.relativeVelocity.sqrMagnitude || x.Velocity <= 0));
            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var angle = Mathf.Atan2(collision.contacts[0].normal.y, collision.contacts[0].normal.x) * Mathf.Rad2Deg + 90;

                    if (!matches[i].RotateToNormal)
                        angle = 0;
                    var effect = Instantiate(matches[i].EffectPrefab, collision.contacts[0].point, Quaternion.Euler(0, 0, angle));
                    if (matches[i].PlayOnce)
                        _conditions.Remove(matches[0]);
                }
            }
        }
    }
}
