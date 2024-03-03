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

    // �������� �ݴ� �������� ������� ����
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
            var nextPosition = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, _speed * Time.deltaTime);    // ������ �̵��� �Ÿ��� maxDistanceDelta�� ���� �ʴ´ٸ� �׸�ŭ �̵�
            transform.position = nextPosition;
            yield return null;
        }
        Destroy(this.gameObject);
        yield return null;
    }
}
