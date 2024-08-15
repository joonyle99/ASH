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
        _soundList.PlaySFX("Blaze", 1f, 1f, true);
    }

    private void OnDisable()
    {
        SoundManager.Instance.StopLoopSFX();
    }
}
