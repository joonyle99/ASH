using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CollisionWithPlayerDetector : MonoBehaviour
{
    bool _isPlayerInCollider = false;

    ICollisionWithPlayerListener [] _listeners;

    private void Awake()
    {
        _listeners = GetComponents<ICollisionWithPlayerListener>();
    }
    private void FixedUpdate()
    {
        if (IsCollidingWithPlayer())
        {
            if (_isPlayerInCollider)
                OnPlayerStay();
            else
                OnPlayerEnter();
            _isPlayerInCollider = true;
        }
        else
        {
            if (_isPlayerInCollider)
                OnPlayerExit();
            _isPlayerInCollider = false;
        }
    }
    bool IsCollidingWithPlayer()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Player"));
        var contacts = new List<Collider2D>();
        GetComponent<Collider2D>().OverlapCollider(filter, contacts);
        foreach (Collider2D contact in contacts)
        {
            if (contact.GetComponent<PlayerBehaviour>() != null)
                return true;
        }
        return false;
    }
    void OnPlayerEnter()
    {
        foreach(var listener in _listeners)
            listener.OnPlayerEnter(SceneContext.Current.Player);
    }
    void OnPlayerExit()
    {
        foreach (var listener in _listeners)
            listener.OnPlayerExit(SceneContext.Current.Player);
    }
    void OnPlayerStay()
    {
        foreach (var listener in _listeners)
            listener.OnPlayerStay(SceneContext.Current.Player);
    }

}
