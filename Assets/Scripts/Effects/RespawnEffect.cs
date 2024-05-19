using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MaterialController))]
public class RespawnEffect : MonoBehaviour
{
    [SerializeField] private Material _respawnMaterial;
    [SerializeField] private float _duration;

    [Space]

    [SerializeField] float _bottomOffset;
    [SerializeField] float _topOffset;

    [field: Space]

    public bool IsEffectDone { get; private set; }

    private Coroutine _respawnCo;
    private MaterialController _materialController;

    private void Awake()
    {
        _materialController = GetComponent<MaterialController>();
    }

    public void Play(float delay = 0f)
    {
        if (_respawnCo != null)
            StopCoroutine(_respawnCo);

        _respawnCo = StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        var minY = transform.position.y - _bottomOffset;
        var maxY = transform.position.y + _topOffset;

        // Respawn Material Initialize
        _materialController.SetMaterial(_respawnMaterial);

        // Respawn Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            var ratio = eTime / _duration;
            var currentHeight = Mathf.Lerp(minY, maxY, ratio);

            _materialController.SetProgress("_Origin", currentHeight);

            yield return null;

            eTime += Time.deltaTime;
        }

        // Init SpriteRenderers
        _materialController.InitMaterial();

        IsEffectDone = true;
    }

    private void OnDrawGizmosSelected()
    {
        const float halfWidth = 1f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y - _bottomOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y - _bottomOffset, 0));
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y + _topOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y + _topOffset, 0));
    }
}
