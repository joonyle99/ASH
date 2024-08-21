using UnityEngine;

[RequireComponent(typeof(DestructEventCaller))]
public class PlaySoundOnDestruct : MonoBehaviour, IDestructionListener
{
    [SerializeField] SoundClipData _soundData;

    public void OnDestruction()
    {
        if (SoundManager.Instance != null && _soundData.Clip != null)
            SoundManager.Instance.PlaySFX_SoundClipData(_soundData);
    }

}
