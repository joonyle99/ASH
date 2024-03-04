using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;

    public void PlayerBattleBGM()
    {
        SoundManager.Instance.PlayCommonBGM(key);
    }
}
