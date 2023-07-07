using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [SerializeField] SoundList _soundList;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _soundList.PlaySFX("hit1");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _soundList.PlaySFX("hit2");
    }
}
