using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] string key2;

    public void PlayBGM()
    {
        SoundManager.Instance.BGMFadeInOut(key, 0f, "", 0f, 2f);
    }

    public void PlayBGM2()
    {
        SoundManager.Instance.BGMFadeInOut(key2, 3, "", 3);
    }
}