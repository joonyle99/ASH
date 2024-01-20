using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PersistentGroupObject : MonoBehaviour
{
    [SerializeField] string _groupName;
    [SerializeField] bool _removeOnDestroy = false;

    private void Awake()
    {
        PersistentDataManager.TryAddDataGroup(_groupName);
    }
    private void OnDestroy()
    {
        if (_removeOnDestroy)
            PersistentDataManager.RemoveDataGroup(_groupName);
    }
}