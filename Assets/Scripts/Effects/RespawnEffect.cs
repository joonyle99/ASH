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

    private MaterialController materialController;

    private void Awake()
    {
        materialController = GetComponent<MaterialController>();
    }

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        var minY = transform.position.y - _bottomOffset;
        var maxY = transform.position.y + _topOffset;

        // Respawn Material Initialize
        materialController.SetMaterial(_respawnMaterial);

        // Respawn Effect Progress
        var eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            var ratio = eTime / _duration;
            var currentHeight = Mathf.Lerp(minY, maxY, ratio);

            materialController.SetProgress("_Origin", currentHeight);
        }

        // Init SpriteRenderers
        materialController.InitMaterial();

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
