using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackPanther_CutScene : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetTriggerAnim(string param)
    {
        _animator.SetTrigger(param);
    }
}
