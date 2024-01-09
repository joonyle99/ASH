using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : InteractableObject
{
    [SerializeField] bool _clampSpeed = false;
    [SerializeField] float _maxRollSpeed;
    [SerializeField] float _pushPower;

    [SerializeField] SoundList _soundList;
    [SerializeField] float _pushSoundInterval;

    Rigidbody2D _rigidbody;
    IAttackListener _attackableComponent;

    bool _isPushSoundPlaying = false;
    float _moveDirection = 0;

    public bool IsBreakable { get { return _attackableComponent == null; } }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _attackableComponent = GetComponent<IAttackListener>();
    }
    protected override void OnInteract()
    {
        Player.MovementController.enabled = true;
        _moveDirection = Player.PlayerLookDir2D.x;
    }
    IEnumerator PlayPushSoundCoroutine(string key, float interval)
    {
        _isPushSoundPlaying = true;
        _soundList.PlaySFX(key);
        yield return new WaitForSeconds(interval);
        _isPushSoundPlaying = false;
    }

    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0.3f && _rigidbody.GetContacts(new Collider2D[1]) > 0)
        {
            if (!_isPushSoundPlaying)
                StartCoroutine(PlayPushSoundCoroutine("Roll", _pushSoundInterval));
        }
    }
    public override void UpdateInteracting()
    {
        //TODO : 플레이어가 돌에서 떨어졌을 때
        if (IsInteractionKeyUp || IsPlayerStateChanged || Player.RawInputs.Movement.x * _moveDirection < 0)
        {
            ExitInteraction();
        }
    }
    public override void FixedUpdateInteracting()
    {
        _rigidbody.AddForce(Player.RawInputs.Movement * _pushPower);
        if(_clampSpeed)
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxRollSpeed);

    }
    protected override void OnInteractionExit()
    {
        Player.MovementController.enabled = false;
    }
}
