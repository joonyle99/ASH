using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] string key2;

    public void PlayBGM()
    {
        SoundManager.Instance.PlayCommonBGM(key);
    }
    public void PlayBGM2()
    {
        SoundManager.Instance.PlayCommonBGM(key2);
    }

    public void StopBGM()
    {
        SoundManager.Instance.StopBGM();
    }
}