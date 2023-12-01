using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePartsOnDestroy : MonoBehaviour
{
    [SerializeField] Transform[] _parts;

    private void OnDestroy()
    {
        foreach (var part in _parts)
        {
            part.parent = null;
            part.gameObject.SetActive(true);
        }
    }
}
