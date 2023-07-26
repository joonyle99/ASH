using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SoundPlayerOnParticleCollision : MonoBehaviour
{
    [SerializeField] float _probability;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Random.Range(0f,1f) < _probability)
            SoundManager.Instance.PlayCommonSFXPitched("SE_CrashRock_split");

    }
}
