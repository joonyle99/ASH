using UnityEngine;

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

        Debug.Log($"Random Range: {_range.Start} ~ {_range.End - 1}");
        Debug.Log($"Percentage: {_percentage}");

        return randomInt <= _percentage;
    }

    public void DropProcess(Vector3 dropPosition)
    {
        Instantiate(_LifePiece, dropPosition, Quaternion.identity);
    }
}
