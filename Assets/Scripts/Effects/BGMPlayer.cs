using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string key;
    private void Start()
    {
        SoundManager.Instance.PlayCommonBGM(key);
    }
}
