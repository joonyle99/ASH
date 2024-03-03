using System.Collections;
using UnityEngine;

public class BabyBearController : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private float _speed;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetTriggerAnim(string paramName)
    {
        _animator.SetTrigger(paramName);
    }

    // 새끼곰이 반대 방향으로 사라지는 연출
    public void RunAway()
    {
        SetTriggerAnim("Walk");
        StartCoroutine(RunAwayCoroutine());
    }

    public IEnumerator RunAwayCoroutine()
    {
        float eTime = 0f;
        while (eTime < _time)
        {
            eTime += Time.deltaTime;
            var nextPosition = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, _speed * Time.deltaTime);    // 앞으로 이동할 거리가 maxDistanceDelta를 넘지 않는다면 그만큼 이동
            transform.position = nextPosition;
            yield return null;
        }
        Destroy(this.gameObject);
        yield return null;
    }
}
