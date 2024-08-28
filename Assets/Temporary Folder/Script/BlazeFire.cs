using UnityEngine;

public class BlazeFire : MonoBehaviour
{
    private AudioSource _loopSound;

    private void Awake()
    {
        _loopSound = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        _loopSound.Play();
    }
    private void OnDisable()
    {
        _loopSound.Stop();
    }
}
