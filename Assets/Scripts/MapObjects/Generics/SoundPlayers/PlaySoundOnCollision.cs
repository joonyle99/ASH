using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            var matches = _conditions.FindAll(x => x.ObjectType == otherObject.Type 
                                                  && ((x.Velocity * x.Velocity) <= collision.relativeVelocity.sqrMagnitude || x.Velocity <= 0));
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
