using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyTools
{
    /// <summary>
    /// This SingletonBehaviour logs null if instance is not created and tries to access it.
    /// If another instance of this is created, the new instance is destroyed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonBehaviourFixed<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                        Debug.LogError("No object of " + typeof(T).Name + " is no found");
                }
                return instance;
            }
        }
        static T instance = null;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }

}