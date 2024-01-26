using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightSwitch : MonoBehaviour
{
    [SerializeField] float _pressThreshold;
    [SerializeField] GameObject [] switchTargets;

    [SerializeField] Collider2D _buttonCollider;
    [SerializeField] SoundList _soundList;

    List<ToggleableObject> _toggleListeners = new List<ToggleableObject>();
    bool _isOn = false;
    private void OnValidate()
    {
        foreach(var target in switchTargets)
        {
            if (target != null && target.GetComponents<ToggleableObject>().Length == 0)
                Debug.LogErrorFormat("Report target object {0} doesn't have a ToggleableObject component", target.name);
        }
    }

    private void Awake()
    {
        foreach (var target in switchTargets)
        {
            _toggleListeners.AddRange(target.GetComponents<ToggleableObject>());
        }
    }

    private void Update()
    {
        float buttonLocalTop = _buttonCollider.bounds.max.y - transform.position.y;
       
        if (!_isOn && buttonLocalTop <= _pressThreshold)
            TurnOn();
        else if (_isOn && buttonLocalTop > _pressThreshold)
            TurnOff();
    }

    void TurnOn()
    {
        //켜지는순간 올라간 물체 정지
        Collider2D[] colliders = new Collider2D[4];
        int count = _buttonCollider.GetContacts(colliders);
        for(int i=0; i<count; i++)
        {
            if (!colliders[i].attachedRigidbody)
                continue;
            colliders[i].attachedRigidbody.velocity = Vector3.zero;
            colliders[i].attachedRigidbody.angularVelocity= 0f;
        }

        _isOn = true;
        foreach (var listener in _toggleListeners)
            listener.TurnOn();

        _soundList.PlaySFX("Press");
    }
    void TurnOff()
    {
        _isOn = false;
        foreach (var listener in _toggleListeners)
            listener.TurnOff();
    }
    private void OnDrawGizmosSelected()
    {
        float buttonHeight = _buttonCollider.bounds.max.y - _buttonCollider.transform.position.y;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + new Vector3(-1f, - _pressThreshold, 0),
                        transform.position + new Vector3(1f, - _pressThreshold, 0));
    }
}
