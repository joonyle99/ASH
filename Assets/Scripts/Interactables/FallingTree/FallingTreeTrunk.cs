using UnityEngine;

/// <summary>
/// ³ª¹« ±âµÕ
/// </summary>
public class FallingTreeTrunk : MonoBehaviour
{
    [SerializeField] SoundList _soundList;
    [SerializeField] float _soundPlayAngle = 10;

    Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    bool _isFallingSoundPlayed = false;
    float _originalRotation;

    public float PushedAngle => Mathf.Abs(Mathf.DeltaAngle(_originalRotation, transform.rotation.eulerAngles.z));

    private void Awake()
    {
        // Debug.Log("falling tree awake");

        _originalRotation = transform.rotation.eulerAngles.z;

        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!_isFallingSoundPlayed)
        {
            if (PushedAngle > _soundPlayAngle)
            {
                _isFallingSoundPlayed = true;
                _soundList.PlaySFX("SE_FallingTree_Break");
            }
        }
    }
}
