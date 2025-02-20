using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] string key2;

    public void PlayBGM()
    {
        SoundManager.Instance.BGMFadeInOut(key, 3, "", 3);
    }

    public void PlayBGM2()
    {
        SoundManager.Instance.BGMFadeInOut(key2, 3, "", 3);
    }
}