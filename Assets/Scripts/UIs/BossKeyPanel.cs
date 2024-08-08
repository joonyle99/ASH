using System;
using UnityEngine;

public class BossKeyPanel : MonoBehaviour
{
    [SerializeField] BossKeySlot[] _keySlots;

    private int _currentKeyCount = 0;
    public int CurrentKeyCount
    {
        get => _currentKeyCount;
        set
        {
            if (value > _keySlots.Length)
                Debug.Log("Key Count is over the limit");

            _currentKeyCount = Math.Clamp(value, 0, _keySlots.Length);
        }
    }

    public void SetKeyCountInstant(int keyCount)
    {
        CurrentKeyCount = keyCount;
        for (int i = 0; i < _keySlots.Length; i++)
        {
            _keySlots[i].SetValue(i < keyCount);
        }
    }
    public void AddKey()
    {
        _keySlots[CurrentKeyCount].Obtain();
        CurrentKeyCount++;
    }
}
