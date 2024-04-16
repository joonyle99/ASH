using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialManager))]
public class RespawnEffect : MonoBehaviour
{
    [SerializeField] private Material _respawnMaterial;
    [SerializeField] private float _duration;

    [Space]

    [SerializeField] float _bottomOffset;
    [SerializeField] float _topOffset;

    [field: Space]

    public bool IsEffectDone { get; private set; }

    private MaterialManager materialManager;

    private float _minY;
    private float _maxY;

    private void Awake()
    {
        materialManager = GetComponent<MaterialManager>();

        _minY = transform.position.y - _bottomOffset;
        _maxY = transform.position.y + _topOffset;
    }

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Respawn Material Initialize
        materialManager.SetMaterialAndProgress(_respawnMaterial, "_Origin", _minY);

        // Respawn Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            var currentHeight = Mathf.Lerp(_minY, _maxY, eTime / _duration);
            materialManager.SetProgress("_Origin", currentHeight);
        }

        yield return new WaitForSeconds(delay);

        IsEffectDone = true;
    }
    public void Revert()
    {
        IsEffectDone = false;

        // Respawn Material Initialize
        materialManager.SetProgress("_Origin", _minY);

        // Init SpriteRenderers
        materialManager.InitMaterial();
    }

    private void OnDrawGizmosSelected()
    {
        float halfWidth = 1f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y - _bottomOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y - _bottomOffset, 0));
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y + _topOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y + _topOffset, 0));
    }
}
