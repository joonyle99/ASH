using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HealthPanelUI : MonoBehaviour
{
    const int MaxLife = 10;

    [SerializeField] Transform[] _lifeIcons;

    public int Life
    {
        set
        {
        }
    }
    void Update()
    {
        var hp = SceneContext.Current.Player.CurHp;
        if (hp > MaxLife)
            hp = MaxLife;
        else if (hp < 0)
            hp = 0;
        UpdateLifeIcons(hp);
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
