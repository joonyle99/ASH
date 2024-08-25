using UnityEngine;

public class DestroyAll : MonoBehaviour
{
    [SerializeField] GameObject []_objects;
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var obj in _objects)
            {
                Destroy(obj);
            }
        }
#endif
    }
}
