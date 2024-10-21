using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public GameObject FireBody;
    public Renderer TornadoRenderer;
    public Renderer TornadoExteriorRenderer;

    [Space]

    public float TargetSpeed = 0.2f;
    public float TargetDuration = 3f;

    private void Awake()
    {
        transform.localScale = new Vector3(0f, 0f, 0f);
    }

    public void TornadoAnimation()
    {
        StartCoroutine(TornadoAnimationCoroutine());
    }
    private IEnumerator TornadoAnimationCoroutine()
    {
        yield return GrowUpEffectCoroutine(TargetDuration);

        yield return new WaitForSeconds(2f);

        yield return GrowDownEffectCoroutine(TargetDuration);
    }

    private IEnumerator GrowUpEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(2.3f, 2.3f, 2.3f);

        // speed
        var material = TornadoRenderer.material;
        var startSpeed = material.GetFloat("_TextureScrollYSpeed");
        var targetSpeed = TargetSpeed;

        while (eTime < duration)
        {
            var t = eTime / duration;

            var nextScale = Vector3.Lerp(startSize, targetSize, t);
            transform.localScale = nextScale;

            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            material.SetFloat("_TextureScrollYSpeed", nextSpeed);

            yield return new WaitForEndOfFrame();

            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;

        material.SetFloat("_TextureScrollYSpeed", targetSpeed);

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
        var startSpeed = material.GetFloat("_TextureScrollYSpeed");
        var targetSpeed = 0f;

        while (eTime < duration)
        {
            var t = eTime / duration;

            var nextScale = Vector3.Lerp(startSize, targetSize, t);
            transform.localScale = nextScale;

            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);
            material.SetFloat("_TextureScrollYSpeed", nextSpeed);

            yield return new WaitForEndOfFrame();

            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;

        material.SetFloat("_TextureScrollYSpeed", targetSpeed);

        TornadoRenderer.gameObject.SetActive(false);
    }
}
