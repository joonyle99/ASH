using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] AudioSource _source;

    public void PlaySound()
    {
        _source.Play();
    }
    public void StopSound()
    {
        _source.Stop();
    }

}
