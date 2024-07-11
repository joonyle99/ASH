using UnityEngine;

/// 리플렉션(reflection) 사용 클래스
/// 런타임에 타입 정보에 접근하고 조작할 수 있는 기능을 제공
/// 
/// ex) 리플렉션 사용 예시
/// 특정 플러그인을 동적으로 로드할 때 사용
/// 비공개 멤버에 접근하여 내부 상태를 검사하거나 테스트
/// ORM(Object-Relational Mapping): 데이터베이스 엔티티와 객체 간의 매핑을 동적으로 처리할 때 사용
/// 객체를 동적으로 직렬화하거나 역직렬화할 때 사용

public class TriggerEnter2DInvoker : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _targetBehaviour;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_targetBehaviour != null)
        {
            // _targetBehaviour의 OnTriggerEnter2D 메서드 호출
            var methodName = "OnTriggerEnter2D";
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var reflectionMethod = _targetBehaviour.GetType().GetMethod(methodName, bindingFlags);
            reflectionMethod?.Invoke(_targetBehaviour, new object[] { collision });
        }
    }
}
