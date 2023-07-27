using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanelUI : MonoBehaviour
{
    const int MaxLife = 10;

    [SerializeField] Slider _healGauge;
    [SerializeField] Transform[] _lifeIcons;

    public float HealGauge { get { return _healGauge.value; } set { _healGauge.value = value; } }

    public int Life
    {
        set
        {
            if (value > MaxLife)
                value = MaxLife;
            else if (value < 0)
                value = 0;
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
        for (int i = 0; i < _lifeIcons.Length; i++)
        {
            bool on = i < target;
            _lifeIcons[i].gameObject.SetActive(on);
        }
    }
}
