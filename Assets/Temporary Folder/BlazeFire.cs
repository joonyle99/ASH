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
        _soundList.PlaySFX("Blaze");
    }
    private void OnDisable()
    {
        // _soundList.StopSFX("Blaze", isLoop: true);
    }
}
