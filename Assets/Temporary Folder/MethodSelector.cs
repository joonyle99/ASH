using System.Collections;
using System.Reflection;
using UnityEngine;

public class MethodSelector : MonoBehaviour
{
    public MonoBehaviour script;

    private void Start()
    {
        MethodInfo[] methodInfos = script.GetType().GetMethods();

        foreach (MethodInfo methodInfo in methodInfos)
        {
            if(methodInfo.ReturnType == typeof(System.Collections.IEnumerator))
            {
                Debug.Log($"<color=red>method name</color>: {methodInfo.Name} / <color=yellow>return type</color>: {methodInfo.ReturnType}");
            }
        }
    }

    public IEnumerator enumerator()
    {
        Debug.Log("enumerator");

        yield return null;
    }
}
