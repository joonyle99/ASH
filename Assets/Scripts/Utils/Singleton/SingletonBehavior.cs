using UnityEngine;

namespace joonyleTools
{
    public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour // 'T'���� UnityEngine.Object���� Boxing ��ȯ�� ����Ǿ�� �Ѵ�.
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // �� ������ �̹� �����ϴ� �ν��Ͻ��� ã�´�
                    _instance = FindObjectOfType<T>();

                    /*
                    // �� ������ �ν��Ͻ��� ã�� ���ߴٸ� ���� �����Ѵ�
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
            // �̹� �ν��Ͻ��� �����Ѵٸ� ���� ������ �ν��Ͻ��� �����Ѵ�.
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            // �ν��Ͻ��� �������� �ʴ´ٸ� ���� ������ �ν��Ͻ��� �Ҵ��Ѵ�.
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}