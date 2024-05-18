using System.Collections.Generic;
using UnityEngine;

public class RollingStone : InteractableObject
{
    [SerializeField] bool _clampSpeed = false;
    [SerializeField] float _maxRollSpeed;
    [SerializeField] float _pushPower;

    [SerializeField] bool _stopOnRelease = false;
    [SerializeField] bool _canPull = false;
    [SerializeField] Collider2D _interactionZone;
    [SerializeField] LayerMask _playerMask;

    [SerializeField] ParticleHelper _dustParticle;

    [SerializeField] SoundList _soundList;
    [SerializeField] float _pushSoundInterval;

    [SerializeField] AudioSource _rollAudio;
    Rigidbody2D _rigidbody;
    IAttackListener _attackableComponent;

    float _rollAudioTiming = 0f;
    float _rollAudioOriginalVolume;

    float _moveDirection = 0;

    public bool IsBreakable { get { return _attackableComponent == null; } }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _attackableComponent = GetComponent<IAttackListener>();
        _rollAudioOriginalVolume = _rollAudio.volume;
    }
    protected override void OnInteract()
    {
        //Player.MovementController.enabled = true;
        _moveDirection = Player.PlayerLookDir2D.x;
    }

    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0.4f && _rigidbody.GetContacts(new Collider2D[1]) > 0)
        {
            if (!_rollAudio.isPlaying)
            {
                _rollAudio.volume = _rollAudioOriginalVolume;
                _rollAudio.Play();

                _dustParticle.Play();
            }
            _rollAudioTiming = 1f;
        }
        else
        {
            _rollAudioTiming -= Time.deltaTime;
            _rollAudio.volume = _rollAudioOriginalVolume * _rollAudioTiming;
            if (_rollAudio.isPlaying && _rollAudioTiming <= 0f)
            {
                _rollAudioTiming = 0f;
                _rollAudio.Stop();

                _dustParticle.Stop();
            }
        }
    }
    public override void UpdateInteracting()
    {
        List<Collider2D> contacts = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = _playerMask;
        filter.useLayerMask= true;
        var contactCount = _interactionZone.OverlapCollider(filter, contacts);
        if (IsInteractionKeyUp || !IsPlayerInteractionState || (!_canPull && Player.RawInputs.Movement.x * _moveDirection < 0) ||
            contactCount == 0)
        {
            ExitInteraction();
        }
    }
    public override void FixedUpdateInteracting()
    {
        if (_canPull && Player.RawInputs.Movement.x * _moveDirection < 0)
            _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower * 0.7f);
        else
            _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower);

        if (Player.RawInputs.Movement.x * _moveDirection > 0)
            Player.Rigidbody.AddForce(Player.RawInputs.Movement * 70);
        if (_clampSpeed)
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxRollSpeed);

    }
    protected override void OnInteractionExit()
    {
        if (_stopOnRelease)
            _rigidbody.velocity *= 0.1f;
        //Player.MovementController.enabled = false;
    }
}
