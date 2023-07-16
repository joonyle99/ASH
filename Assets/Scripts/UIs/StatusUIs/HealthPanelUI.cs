using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelUI : MonoBehaviour
{
    const int MaxLife = 10;

    [SerializeField] Slider _healGauge;
    [SerializeField] Transform[] _lifeIcons;
    int _lastLifeIconIndex = 9;

    public float HealGauge { get { return _healGauge.value; } set { _healGauge.value = value; } }

    public int Life
    {
        get
        {
            return _lastLifeIconIndex + 1;
        }
        set
        {
            if (value > MaxLife)
                value = MaxLife;
            else if (value < 0)
                value = 0;
            if (_lastLifeIconIndex != value - 1)
                UpdateLifeIcons(value);
        }
    }
    void Awake()
    {
    }
    void UpdateLifeIcons(int target)
    {
        if (target <= 0)
        {
            //»ç¸Á
        }
        if(target < _lastLifeIconIndex + 1)
        {
            for (int i = _lastLifeIconIndex; i > target - 1; i--)
            {
                _lifeIcons[i].gameObject.SetActive(false);
            }
        }
        else if (target > _lastLifeIconIndex + 1)
        {
            for (int i = _lastLifeIconIndex; i < target; i++)
            {
                _lifeIcons[i].gameObject.SetActive(true);
            }
        }
        _lastLifeIconIndex = target - 1;
    }
}
