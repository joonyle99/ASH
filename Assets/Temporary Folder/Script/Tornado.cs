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
        yield return GrowUpEffectCoroutine(2f);

        yield return new WaitForSeconds(2f);

        yield return GrowDownEffectCoroutine(2f);
    }

    private IEnumerator GrowUpEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(2.3f, 2.3f, 2.3f);

        // speed
        var material = TornadoRenderer.material;
        var startSpeed = material.GetFloat("_ShapeYSpeed");
        var targetSpeed = TargetShapeSpeed;

        while (eTime < duration)
        {
            var t = eTime / duration;

            // size
            var nextScale = Vector3.Lerp(startSize, targetSize, t);
            transform.localScale = nextScale;

            // speed
            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            material.SetFloat("_ShapeYSpeed", nextSpeed);

            yield return null;
            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;
        material.SetFloat("_ShapeYSpeed", targetSpeed);

        FireBody.gameObject.SetActive(true);
    }
    private IEnumerator GrowDownEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(0f, 0f, 0f);

        // speed
        var material = TornadoRenderer.material;
        var startSpeed = material.GetFloat("_ShapeYSpeed");
        var targetSpeed = 0f;

        material.SetFloat("_ShapeYSpeed", 0f);

        while (eTime < duration)
        {
            var t = eTime / duration;

            // size
            transform.localScale = Vector3.Lerp(startSize, targetSize, t);

            // speed
            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            material.SetFloat("_ShapeYSpeed", nextSpeed);

            yield return null;
            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;
        material.SetFloat("_ShapeYSpeed", targetSpeed);

        TornadoRenderer.gameObject.SetActive(false);
    }
}
