using System.Collections;
using UnityEngine;

/// <summary>
/// 일정 확률로 생명 조각을 드랍하도록 하는 클래스
/// </summary>
public class LifePieceDropper : MonoBehaviour
{
    [SerializeField] private LifePiece _LifePiece;
    [SerializeField] private int _percentage;

    private Range _range;

    private void Awake()
    {
        _range = new Range(1, 101);
    }

    public bool IsDropChance()
    {
        int randomInt = (int)_range.Random();

        // Debug.Log($"Random Range: {_range.Start} ~ {_range.End - 1}");
        // Debug.Log($"Percentage: {_percentage}");

        return randomInt <= _percentage;
    }

    public void DropProcess(Vector3 dropPosition)
    {
        StartCoroutine(DropProcessCoroutine(dropPosition, 1f));
    }

    private IEnumerator DropProcessCoroutine(Vector3 dropPosition, float dropDelay)
    {
        yield return new WaitForSeconds(dropDelay);

        // 생명 조각 생성
        Instantiate(_LifePiece, dropPosition, Quaternion.identity);
    }
}
