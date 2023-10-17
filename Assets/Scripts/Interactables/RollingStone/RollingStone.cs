using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RollingStone : ContinuousInteractableObject
{
    Rigidbody2D _rigidbody;

    RollingStonePlayerInteractor _playerInteractor;

    [SerializeField] protected float _threatVelocityThreshold = 3;
    [SerializeField] protected float _damage = 1;

    PolygonCollider2D _collider;

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
        
        if (_isInteracting)
        {
            if (!IsPlayerColliding())
            {
                SceneContext.Current.Player.InteractionController.RelaseInteractingObject();
            }
        }

    }

    bool IsPlayerColliding()
    {
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        _rigidbody.GetContacts(contacts);
        bool playerCollided = false;
        foreach (ContactPoint2D contact in contacts)
        {
            if (contact.rigidbody != null && contact.rigidbody.GetComponent<PlayerBehaviour>() != null)
            {
                playerCollided = true;
                break;
            }
        }
        return playerCollided;
    }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
        _playerInteractor = GetComponentInChildren<RollingStonePlayerInteractor>();
        _playerInteractor.ThreatVelocityThreshold = _threatVelocityThreshold;
        _playerInteractor.Damage = _damage;
        _immovable = true;
    }

#if UNITY_EDITOR
    public void ApplyShape()
    {
        GetComponentInChildren<RollingStonePlayerInteractor>().GetComponent<PolygonCollider2D>().points 
            = GetComponent<PolygonCollider2D>().points;

    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(RollingStone))]
public class CubeGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RollingStone t = (RollingStone)target;
        if (GUILayout.Button("Apply Shape"))
        {
            t.ApplyShape();
        }
    }
}
#endif