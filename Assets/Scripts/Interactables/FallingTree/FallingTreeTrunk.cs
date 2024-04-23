using UnityEngine;

/// <summary>
/// ³ª¹« ±âµÕ
/// </summary>
public class FallingTreeTrunk : MonoBehaviour
{
    [SerializeField] SoundList _soundList;

    Rigidbody2D _rigidbody;

    bool _isFallingSoundPlayed = false;
    [SerializeField] float _soundPlayAngle = 10;
    float _originalRotation;

    public Rigidbody2D Rigidbody { get {  return _rigidbody; } }
    public float PushedAngle => Mathf.Abs(Mathf.DeltaAngle(_originalRotation, transform.rotation.eulerAngles.z));
    private void Awake()
    {
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
