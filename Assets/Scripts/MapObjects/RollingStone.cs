using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingStone : ContinuousInteractableObject
{
    Rigidbody2D _rigidbody;

    RollingStonePlayerInteractor _playerInteractor;

    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damage = 1;

    bool _immovable
    {
        get { return _playerInteractor.isActiveAndEnabled; }
        set
        {
            _playerInteractor.gameObject.SetActive(value);
            if (value)
                gameObject.layer = LayerMask.NameToLayer("ExceptPlayer");
            else
                gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    bool _isInteracting = false;
    public override void InteractEnter()
    {
        Debug.Log("InteractEnter");
        _immovable = false;
        _isInteracting = true;
    }

    public override void InteractExit()
    {
        Debug.Log("InteractExit");
        _immovable = true;
        _isInteracting = false;
    }

    public override void InteractUpdate()
    {
        /*
        if (_isInteracting)
        {
            var mask = LayerMask.GetMask(new string[] { "Player" });
            print(mask);
            if (!_playerInteractor.Collider.IsTouchingLayers())
                SceneContext.Current.Player.InteractionController.RelaseInteractingObject();
        }
        */
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerInteractor = GetComponentInChildren<RollingStonePlayerInteractor>();
        _playerInteractor.ThreatVelocityThreshold = _threatVelocityThreshold;
        _playerInteractor.Damage = _damage;
        _immovable = true;
    }


}
