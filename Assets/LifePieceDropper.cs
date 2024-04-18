using UnityEngine;

public class LifePieceDropper : MonoBehaviour
{
    [SerializeField] private LifePiece _LifePiece;
    [SerializeField] private int _percentage;

    public bool IsDropChance()
    {
        var randomInt = Random.Range(1, 101);
        return randomInt <= _percentage;
    }

    public void DropProcess(Vector3 dropPosition)
    {
        Instantiate(_LifePiece, dropPosition, Quaternion.identity);
    }
}
