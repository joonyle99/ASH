using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public GameObject FireBody;
    public Renderer TornadoRenderer;

    [Space]

    public float TargetShapeSpeed = 0.2f;

    public void TornadoAnimation()
    {
        StartCoroutine(TornadoAnimationCoroutine());
    }
    private IEnumerator TornadoAnimationCoroutine()
    {
        yield return GrowUpEffectCoroutine(2.5f);

        yield return new WaitForSeconds(2f);

        yield return GrowDownEffectCoroutine(2.5f);
    }

    private IEnumerator GrowUpEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(2.3f, 2.3f, 2.3f);

        // speed
        var material = TornadoRenderer.material;
        material.SetFloat("_ShapeYSpeed", TargetShapeSpeed);

        while (eTime < duration)
        {
            var t = eTime / duration;

            var nextScale = Vector3.Lerp(startSize, targetSize, t);
            transform.localScale = nextScale;

            yield return null;

            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;

        FireBody.gameObject.SetActive(true);
    }
    private IEnumerator GrowDownEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(0f, 0f, 0f);

        while (eTime < duration)
        {
            var t = eTime / duration;

            var nextScale = Vector3.Lerp(startSize, targetSize, t);
            transform.localScale = nextScale;

            yield return null;

            eTime += Time.deltaTime;
        }

        // speed
        var material = TornadoRenderer.material;
        material.SetFloat("_ShapeYSpeed", 0f);

        transform.localScale = targetSize;

        TornadoRenderer.gameObject.SetActive(false);
    }
}
