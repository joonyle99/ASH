using System.Collections;
using UnityEngine;

public class DisintegrateEffect_Old : MonoBehaviour
{
    [SerializeField] Material _disintegrateMaterial;
    [SerializeField] float _duration;
    [SerializeField] float _timeOffsetAfterParticle = 0.2f;
    [SerializeField] ParticleHelper _particle;

    [SerializeField] float _bottomOffset;
    [SerializeField] float _topOffset;

    // [ContextMenuItem("Get all", "GetAllSpriteRenderers")]
    [SerializeField] SpriteRenderer [] _spriteRenderers;

    public bool IsEffectDone { get; private set; } = false;
    public float Duration => _duration;

    /*
    void GetAllSpriteRenderers()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }
    */

    public void Play(float delay = 0f)
    {
        StartCoroutine(ProgressCoroutine(delay));
    }
    IEnumerator ProgressCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Disintegrate Effect Initialize
        foreach (var renderer in _spriteRenderers)
        {
            renderer.material = _disintegrateMaterial;
            renderer.material.SetFloat("_Progress", 0);
            renderer.material.SetFloat("_MinY", transform.position.y - _bottomOffset);
            renderer.material.SetFloat("_MaxY", transform.position.y + _topOffset);
        }

        // Particle Effect
        _particle.transform.parent = null;
        _particle.transform.position = transform.position;
        _particle.gameObject.SetActive(true);

        yield return new WaitForSeconds(_timeOffsetAfterParticle);

        // Disintegrate Effect Progress
        float eTime = 0f;
        while (eTime < _duration)
        {
            yield return null;
            foreach (var renderer in _spriteRenderers)
            {
                renderer.material.SetFloat("_Progress", eTime / _duration);
                // renderer.material.SetFloat("_MinY", transform.position.y - _bottomOffset);
                // renderer.material.SetFloat("_MaxY", transform.position.y + _topOffset);
            }
            eTime += Time.deltaTime;
        }

        IsEffectDone = true;
    }

    private void OnDrawGizmosSelected()
    {
        float halfWidth = 1f;

        Gizmos.color = Color.grey;
        Gizmos.DrawLine(new Vector3(transform.position .x - halfWidth, transform.position.y - _bottomOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y - _bottomOffset, 0));
        Gizmos.DrawLine(new Vector3(transform.position.x - halfWidth, transform.position.y + _topOffset, 0), new Vector3(transform.position.x + halfWidth, transform.position.y + _topOffset, 0));
    }
}
