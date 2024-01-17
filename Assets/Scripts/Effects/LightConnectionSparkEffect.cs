using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightConnectionSparkEffect : MonoBehaviour
{
    [SerializeField] float _moveSpeed;
    [SerializeField] float _interval;
    [SerializeField] Range _randomIntervalOffset;

    LanternLike _a;
    LanternLike _b;
    Transform _nextTarget;
    ParticleHelper _particle;
    
    public void SetConnection(LanternLike a, LanternLike b)
    {
        _a = a;
        _b = b;
        _nextTarget = a.transform;
    }
    private void Awake()
    {
        _particle = GetComponentInChildren<ParticleHelper>();
    }
    IEnumerator TravelCoroutine()
    {
        _particle.gameObject.SetActive(false);
        bool isATarget = _nextTarget == _a.transform;
        if ( isATarget)
            transform.position = _b.transform.position;
        else
            transform.position = _a.transform.position;

        _particle.gameObject.SetActive(true);
        while (true)
        {
            Vector3 moveDir = _nextTarget.position - transform.position;
            if (Vector3.SqrMagnitude(moveDir) < 0.01f)
                break;

            _particle.SetStartRotation(-Mathf.Atan2(moveDir.y, moveDir.x) + Mathf.PI / 2);
            transform.position = Vector3.MoveTowards(transform.position, _nextTarget.position, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (isATarget)
            _nextTarget = _b.transform;
        else
            _nextTarget = _a.transform;
        yield return new WaitForSeconds(_interval + _randomIntervalOffset.Random());
        StartCoroutine(TravelCoroutine());
    }
    void OnEnable()
    {
        if (_nextTarget != null) 
            StartTravel();
    }
    public void StartTravel()
    {
        StartCoroutine(TravelCoroutine());
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
}
