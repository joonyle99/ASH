using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestructEventCaller))]
public class EnablePartsOnDestruct : MonoBehaviour, IDestructionListener
{
    [SerializeField] Transform[] _parts;

    public void OnDestruction()
    {
        foreach (var part in _parts)
        {
            part.parent = null;
            part.gameObject.SetActive(true);
        }
    }
}
