using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBeam : MonoBehaviour, ITriggerListener
{
    [SerializeField] BoxCollider2D _triggerBox;
    [SerializeField] DarkBeamEffect _effect;

    [SerializeField] float _damage;
    [SerializeField] float _maxLength;
    [SerializeField] LayerMask _obstacleLayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(rotation), Mathf.Sin(rotation), 0);
        var hit = Physics2D.Raycast(transform.position, direction, _maxLength, _obstacleLayer);
        float length = _maxLength;
        if (hit.transform != null)
            length = hit.distance;

        _effect.RecreateMesh(length);

        _triggerBox.offset = length * Vector3.right / 2;
        _triggerBox.size = new Vector2(length, _triggerBox.size.y);
    }
    public void OnEnterReported(TriggerActivator activator, TriggerReporter reporter)
    {
        if (activator.Type == ActivatorType.Player)
        {
            activator.GetComponent<PlayerBehaviour>().TriggerInstantRespawn(_damage);
        }
    }
}
