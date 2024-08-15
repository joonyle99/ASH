using UnityEngine;

public class BlazeFire : MonoBehaviour
{
    private SoundList _soundList;

    private void Awake()
    {
        _soundList = GetComponent<SoundList>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }
}
