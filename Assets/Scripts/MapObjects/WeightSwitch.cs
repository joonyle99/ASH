using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightSwitch : MonoBehaviour
{
    [SerializeField] Rigidbody2D _button;

    private void FixedUpdate()
    {
        _button.position = _button.transform.position;
    }
}
