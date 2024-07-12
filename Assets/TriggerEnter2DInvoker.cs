using UnityEngine;

/// ���÷���(reflection) ��� Ŭ����
/// ��Ÿ�ӿ� Ÿ�� ������ �����ϰ� ������ �� �ִ� ����� ����
/// 
/// ex) ���÷��� ��� ����
/// Ư�� �÷������� �������� �ε��� �� ���
/// ����� ����� �����Ͽ� ���� ���¸� �˻��ϰų� �׽�Ʈ
/// ORM(Object-Relational Mapping): �����ͺ��̽� ��ƼƼ�� ��ü ���� ������ �������� ó���� �� ���
/// ��ü�� �������� ����ȭ�ϰų� ������ȭ�� �� ���

public class TriggerEnter2DInvoker : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _targetBehaviour;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_targetBehaviour != null)
        {
            // _targetBehaviour�� OnTriggerEnter2D �޼��� ȣ��
            var methodName = "OnTriggerEnter2D";
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var reflectionMethod = _targetBehaviour.GetType().GetMethod(methodName, bindingFlags);
            reflectionMethod?.Invoke(_targetBehaviour, new object[] { collision });
        }
    }
}
