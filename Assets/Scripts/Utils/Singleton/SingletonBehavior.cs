using UnityEngine;

namespace joonyleTools
{
    public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour // 'T'에서 UnityEngine.Object로의 Boxing 변환이 보장되어야 한다.
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 씬 내에서 이미 존재하는 인스턴스를 찾는다
                    _instance = FindObjectOfType<T>();

                    /*
                    // 씬 내에서 인스턴스를 찾지 못했다면 새로 생성한다
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                        DontDestroyOnLoad(singletonObject);
                    }
                    */
                }

                return _instance;
            }
        }

        private static T _instance = null;

        protected virtual void Awake()
        {
            // 이미 인스턴스가 존재한다면 새로 생성된 인스턴스를 삭제한다.
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            // 인스턴스가 존재하지 않는다면 새로 생성된 인스턴스를 할당한다.
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}