using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKeyPanel : MonoBehaviour
{
    [SerializeField] BossKeySlot[] _keySlots;

    int _currentKeyCount = 0;

    public void SetKeyCountInstant(int keyCount)
    {
        _currentKeyCount = keyCount;
        for (int i=0; i<_keySlots.Length; i++)
        {
            _keySlots[i].SetValue(i < keyCount);
        }
    }
    public void AddKey()
    {
        _keySlots[_currentKeyCount].Obtain();
        _currentKeyCount++;
    }
}
