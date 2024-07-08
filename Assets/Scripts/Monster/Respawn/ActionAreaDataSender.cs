using UnityEngine;

public abstract class ActionAreaDataSender : MonoBehaviour
{
    [Header("ActionArea Data Sender")]
    [Space]

    [SerializeField]
    protected MonsterBehaviour receiver;

    protected void Start()
    {
        // 몬스터 프리팹 생성 시, 영역 데이터를 업데이트한다
        // floating area의 경우 이 과정에서 navMeshSurface를 bake한다
        UpdateActionAreaData();
    }

    public abstract void UpdateActionAreaData();
    public abstract void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2);
}
