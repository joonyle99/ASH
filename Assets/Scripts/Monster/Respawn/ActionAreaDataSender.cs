using System.Collections;
using UnityEngine;

public abstract class ActionAreaDataSender : MonoBehaviour
{
    [Header("ActionArea Data Sender")]
    [Space]

    [SerializeField]
    protected MonsterBehaviour receiver;

    protected virtual void Awake()
    {

    }
    private void Start()
    {
        // Monster Respawn Manager보다 먼저 행동 반경 데이터를 업데이트한다
        StartCoroutine(UpdateActionAreaDataCoroutine());
    }

    public abstract void ExtractActionAreaInfo(out BoxCollider2D boxCollider1, out BoxCollider2D boxCollider2);
    public abstract void SetActionAreaPosition(Vector3 position1, Vector3 position2);
    public abstract void SetActionAreaScale(Vector3 scale1, Vector3 scale2);

    /// <summary>
    /// Receiver에게 전달하기 위한 행동 반경 데이터를 업데이트한다
    /// </summary>
    public abstract void UpdateActionAreaData();

    private IEnumerator UpdateActionAreaDataCoroutine()
    {
        yield return new WaitForEndOfFrame();
        UpdateActionAreaData();
    }
}
