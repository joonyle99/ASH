using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Auto Updated")]

    [SerializeField] private MonsterBehaviour _owner;

    [SerializeField] private bool _isAnyGroundChecked = false;
    public bool IsAnyGroundChecked => _isAnyGroundChecked;

    [Space(10)]

    [Header("Setting")]

    [SerializeField] private float _checkTerm;

    [SerializeField] private LayerMask _checkLayer;

    [SerializeField] private Coroutine _groundCheckCoroutine;
    [Space(10)]

    [Header("Checker")]
    [SerializeField] private Transform _forwardTransform;
    [SerializeField] private float _forwardDistance;

    private void Awake()
    {
        _owner = GetComponent<MonsterBehaviour>();
    }

    private void Start()
    {
        _groundCheckCoroutine = StartCoroutine(CheckGroundCoroutine());
    }

    private void OnDestroy()
    {
        StopCoroutine(_groundCheckCoroutine);
    }

    private IEnumerator CheckGroundCoroutine()
    {
        while(true)
        {
            yield return null;

            bool flag = false;

            flag = CastRayForward();

            _isAnyGroundChecked = flag;

            yield return new WaitForSeconds(_checkTerm);
        }
    }

    private bool CastRayForward()
    {
        RaycastHit2D hit = Physics2D.Raycast(_forwardTransform.position,
            new Vector2(_owner.RecentDir, 0),
            _forwardDistance,
            _checkLayer);
        //Debug.DrawRay(_forwardTransform.position, new Vector2(_owner.RecentDir, 0) * _forwardDistance, Color.red, 2.0f);
        
        if (hit.collider != null)
            return true;

        return false;
    }
}
