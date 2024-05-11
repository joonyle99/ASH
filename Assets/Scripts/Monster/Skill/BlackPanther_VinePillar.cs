using System.Collections;
using UnityEngine;

public class BlackPanther_VinePillar : Monster_IndependentSkill
{
    [Header("VinePillar")]
    [Space]

    public Renderer[] effectRenderers;
    public Collider2D hitCollider;

    public void Opacity(float duration = 0.5f)
    {
        StartCoroutine(OpacityCoroutine(duration));
    }

    private IEnumerator OpacityCoroutine(float duration)
    {
        var eTime = 0f;
        while (eTime < duration)
        {
            yield return null;
            eTime += Time.deltaTime;

            var ratio = eTime / duration;

            if (!hitCollider.enabled && ratio > 0.4f)
            {
                hitCollider.enabled = true;
            }

            foreach (var effectRenderer in effectRenderers)
            {
                effectRenderer.material.SetFloat("_Alpha", ratio);
            }
        }
    }
}
