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
            _currentKeyCount = Math.Clamp(value, 0, BossDungeonManager.Instance.MaxKeyCount);   // CurrentKeyCount는 MaxKeyCount보다 작아야 한다
            _currentKeyCount = Math.Clamp(value, 0, _keySlots.Length - 1);                      // CurrentKeyCount는 KeySlots의 길이보다 작아야 한다

            // TODO: _currentKeyCount와 global data의 bossKeyCount의 개수가 다를 수 있다
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
