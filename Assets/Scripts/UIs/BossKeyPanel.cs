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
            _currentKeyCount = Math.Clamp(value, 0, BossDungeonManager.Instance.MaxKeyCount);   // CurrentKeyCount�� MaxKeyCount���� �۾ƾ� �Ѵ�
            _currentKeyCount = Math.Clamp(value, 0, _keySlots.Length - 1);                      // CurrentKeyCount�� KeySlots�� ���̺��� �۾ƾ� �Ѵ�

            // TODO: _currentKeyCount�� global data�� bossKeyCount�� ������ �ٸ� �� �ִ�
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
