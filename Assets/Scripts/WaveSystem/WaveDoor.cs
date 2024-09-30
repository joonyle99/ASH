using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDoor : MonoBehaviour
{
    [SerializeField] private GameObject _model;

    public void ToggleDoor(bool value)
    {
        _model.SetActive(value);
    }
}
