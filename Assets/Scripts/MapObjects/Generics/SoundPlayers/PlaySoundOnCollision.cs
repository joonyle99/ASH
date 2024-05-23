using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    [System.Serializable]
    public struct CollisionCondition
    {
        public bool PlayOnce;
        public float Velocity;
        public ObjectType ObjectType;
        public string Key;
    }

    [SerializeField] SoundList _soundList;
    [SerializeField] bool _logVelocity = false;

    [Space]

    [Header("Velocity가 높은 것이 앞에 오게 해주세요")]
    [SerializeField] List<CollisionCondition> _conditions;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 큰 돌이랑 충돌 시 쓰러짐
        var otherObject = collision.gameObject.GetComponent<ASHObject>();

        if (otherObject != null)
        {
            if (_logVelocity)
                Debug.Log("Collision with " + otherObject.Type + ": " + collision.relativeVelocity.magnitude);

            var matches = _conditions.FindAll(targetObject => targetObject.ObjectType == otherObject.Type
                                                   && ((targetObject.Velocity * targetObject.Velocity) <= collision.relativeVelocity.sqrMagnitude || targetObject.Velocity <= 0f));

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    _soundList.PlaySFX(matches[i].Key);

                    if (i + 1 < matches.Count && !matches[i + 1].Velocity.Equals(matches[i].Velocity))
                        break;
                }

                if (matches[0].PlayOnce)
                {
                    _conditions.Remove(matches[0]);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 큰 돌이랑 충돌 시 쓰러짐
        var otherObject = collision.gameObject.GetComponent<ASHObject>();

        if (otherObject != null)
        {
            if (_logVelocity)
                Debug.Log("Trigger with " + otherObject.Type + ": " + collision.attachedRigidbody.velocity.magnitude);

            var matches = _conditions.FindAll(x => x.ObjectType == otherObject.Type
                                                  && ((x.Velocity * x.Velocity) <= collision.attachedRigidbody.velocity.sqrMagnitude || x.Velocity <= 0));

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    _soundList.PlaySFX(matches[i].Key);

                    if (i + 1 < matches.Count && !matches[i + 1].Velocity.Equals(matches[i].Velocity))
                        break;
                }

                if (matches[0].PlayOnce)
                {
                    _conditions.Remove(matches[0]);
                }
            }
        }
    }
}
