using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ³ª¹« ±âµÕ
/// </summary>
public class FallingTreeTrunk : MonoBehaviour
{
    [SerializeField] private float _soundPlayAngle;
    [SerializeField] SoundList _soundList;

    Rigidbody2D _rigidbody;

    bool _isCrashSoundPlayed = false;
    bool _isFallingSoundPlayed = false;
    bool _isLandingSoundPlayed = false;

    Quaternion _originalRotation;
    public float FallenAngle { get { return Quaternion.Angle(transform.rotation, _originalRotation); } }
    public Rigidbody2D Rigidbody { get {  return _rigidbody; } }
    private void Awake()
    {
        _originalRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
    }

    void Update()
    {
        if (FallenAngle > _soundPlayAngle)
        {
            if (!_isFallingSoundPlayed)
            {
                _isFallingSoundPlayed = true;
                _soundList.PlaySFX("SE_FallingTree_Break");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Å« µ¹ÀÌ¶û Ãæµ¹ ½Ã ¾²·¯Áü
        if (!_isCrashSoundPlayed && collision.gameObject.GetComponent<RollingStone>())
        {
            _isCrashSoundPlayed = true;
            _soundList.PlaySFX("SE_FallingTree_Collision");
        }

        if (!_isLandingSoundPlayed)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _soundList.PlaySFX("SE_FallingTree_Landing");
                _isLandingSoundPlayed = true;
            }
        }
    }
}
