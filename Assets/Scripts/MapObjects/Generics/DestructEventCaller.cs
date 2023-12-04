using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Must have : [RequireComponent(typeof(DestructEventCaller))]
/// DestructEventCaller will call OnDestruction() when object is destructed.
/// </summary>
public interface IDestructionListener
{
    public void OnDestruction();
}
public class DestructEventCaller : MonoBehaviour
{
    IDestructionListener[] _listeners; 
    private void Awake()
    {
        _listeners = GetComponentsInChildren<IDestructionListener>();
    }
    public void InvokeDestruct()
    {
        foreach(var listener in _listeners) 
        {
            listener.OnDestruction();
        }
    }
}

public static class Destruction
{
    public static void Destruct(GameObject gameObject)
    {
        gameObject.GetComponentInChildren<DestructEventCaller>().InvokeDestruct();
        GameObject.Destroy(gameObject);
    }
    public static void Destruct(DestructEventCaller destructor)
    {
        destructor.InvokeDestruct();
        GameObject.Destroy(destructor.gameObject);
    }
}
