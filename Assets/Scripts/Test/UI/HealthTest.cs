using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTest : MonoBehaviour
{
    [SerializeField] int _lifeSpeed = 1;
    HealthPanelUI _healthPanel;


    private void Awake()
    {
        _healthPanel = GetComponent<HealthPanelUI>();
        _healthPanel.Life = 5;
    }
    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.UpArrow))
            _healthPanel.Life += _lifeSpeed;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _healthPanel.Life -= _lifeSpeed;

        if (Input.GetKey(KeyCode.LeftArrow))
            _healthPanel.HealGauge -= Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            _healthPanel.HealGauge += Time.deltaTime;*/
    }
}
