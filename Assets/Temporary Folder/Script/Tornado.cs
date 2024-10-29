using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public GameObject BlazeFire;
    public GameObject FireBody;

    public Renderer TornadoRenderer;
    public Renderer TornadoExteriorRenderer;

    [Space]

    public Vector3 TargetSize = new Vector3(2.3f, 2.3f, 2.3f);
    public float TargetSpeed = 0.2f;
    public float TargetDuration = 3f;

    [Space]

    public float TornadoWaitTime = 2f;

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

        yield return new WaitForSeconds(TornadoWaitTime);

        yield return GrowDownEffectCoroutine(TargetDuration);
    }

    private IEnumerator GrowUpEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = TargetSize;

        // speed
        var material = TornadoRenderer.material;
        var startSpeed = material.GetFloat("_TextureScrollYSpeed");
        var targetSpeed = TargetSpeed;

        while (eTime < duration)
        {
            var t = eTime / duration;
            var easeOutT = joonyle99.Math.EaseOutQuad(t);

            var nextScale = Vector3.Lerp(startSize, targetSize, easeOutT);
            transform.localScale = nextScale;

            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, easeOutT);
            material.SetFloat("_TextureScrollYSpeed", nextSpeed);

            yield return new WaitForEndOfFrame();

            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;

        material.SetFloat("_TextureScrollYSpeed", targetSpeed);

        BlazeFire.SetActive(false);
        FireBody.SetActive(true);
    }
    private IEnumerator GrowDownEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = Vector3.zero;

        // speed
        var material = TornadoRenderer.material;
        var startSpeed = material.GetFloat("_TextureScrollYSpeed");
        var targetSpeed = 0f;

        while (eTime < duration)
        {
            var t = eTime / duration;
            var easeOutT = joonyle99.Math.EaseOutQuad(t);

            var nextScale = Vector3.Lerp(startSize, targetSize, easeOutT);
            transform.localScale = nextScale;

            var nextSpeed = Mathf.Lerp(startSpeed, targetSpeed, easeOutT);
            material.SetFloat("_TextureScrollYSpeed", nextSpeed);

            yield return new WaitForEndOfFrame();

            eTime += Time.deltaTime;
        }

        transform.localScale = targetSize;

        material.SetFloat("_TextureScrollYSpeed", targetSpeed);

        TornadoRenderer.gameObject.SetActive(false);
    }
}
