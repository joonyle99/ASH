using UnityEngine;

public class SpawnEffectOnDestruct : MonoBehaviour, IDestructionListener
{
    [SerializeField] GameObject _effectPrefab;
    public void OnDestruction()
    {
        Instantiate(_effectPrefab, transform.position, Quaternion.identity);
    }
}
