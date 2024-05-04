using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> _keys = new List<TKey>();
    [SerializeField] private List<TValue> _values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        this.Clear();

        if (_keys.Count != _values.Count)
            throw new Exception("there are different numbers of keys and values");

        for (int i = 0; i < _keys.Count; i++)
            this[_keys[i]] = _values[i];
    }

    public void OnAfterDeserialize()
    {

    }
}
