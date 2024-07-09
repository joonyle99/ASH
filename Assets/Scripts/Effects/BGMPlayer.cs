using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;

    public void PlayBGM()
    {
        SoundManager.Instance.PlayCommonBGM(key);
    }
}