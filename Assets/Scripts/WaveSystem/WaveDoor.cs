using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDoor : Door
{
    [Header("Model")]

    [SerializeField] private GameObject _model;

    public void CloseImmediately()
    {
        if(_animator)
        {
            _animator.SetTrigger("CloseImmediately");
        }
    }
}
