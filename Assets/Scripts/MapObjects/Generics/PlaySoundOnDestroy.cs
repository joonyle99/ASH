using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnDestroy : MonoBehaviour
{
    [SerializeField] SoundClipData _soundData;

    private void OnDestroy()
    {
        if (SoundManager.Instance != null && _soundData.Clip != null)
            SoundManager.Instance.PlaySFX(_soundData);
    }
}
