using UnityEngine;

/// <summary>
/// 나무 기둥
/// </summary>
public class FallingTreeTrunk : MonoBehaviour
{
    [SerializeField] SoundList _soundList;
    [SerializeField] float _soundPlayAngle = 10;

    Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    private PreserveState _statePreserver;

    [SerializeField] private GameObject _fence;

    bool _isFallingSoundPlayed = false;
    float _originalRotation;

    public float PushedAngle => Mathf.Abs(Mathf.DeltaAngle(_originalRotation, transform.rotation.eulerAngles.z));

    private void Awake()
    {
        // Debug.Log("falling tree awake");

        _originalRotation = transform.rotation.eulerAngles.z;

        _rigidbody = GetComponent<Rigidbody2D>();

        _statePreserver = GetComponentInParent<PreserveState>();

        if( _statePreserver)
            _isFallingSoundPlayed = _statePreserver.LoadState<bool>("_isFallingSoundPlayed", false);
    }

    private void Update()
    {
        if (!_isFallingSoundPlayed)
        {
            if (PushedAngle > _soundPlayAngle)
            {
                _isFallingSoundPlayed = true;
                _soundList.PlaySFX("SE_FallingTree_Break");

                if(_statePreserver)
                {
                    _statePreserver.SaveState<bool>("_isFallingSoundPlayed", true);
                }
            }
        }
    }

    public void RemoveFence()
    {
        _fence?.gameObject.SetActive(false);
    }
}
