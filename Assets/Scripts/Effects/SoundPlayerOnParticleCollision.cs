using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SoundPlayerOnParticleCollision : ParticleHelper
{
    [Header("Sound Effects")]
    [SerializeField] string _collisionSoundKey = "";
    [SerializeField] float _collisionSoundProbability = 1f;

    SoundList _soundList;

    new void Awake()
    {
        base.Awake();
        _soundList = GetComponent<SoundList>();
        var collisionModule = ParticleSystem.collision;
        collisionModule.sendCollisionMessages = true;
    }
    private void OnParticleCollision(GameObject other)
    {
        if (_soundList != null && _collisionSoundKey != "")
        {
            if (_collisionSoundProbability > Random.value)
                _soundList.PlaySFX(_collisionSoundKey);
        }

    }
}
