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

    private enum AnimationState { None, GrowingUp, Waiting, GrowingDown }
    private AnimationState currentState = AnimationState.None;

    private float elapsedTime;
    private Vector3 startSize;
    private Vector3 targetSize;
    private float startSpeed;
    private float targetSpeed;
    private Material material;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
        material = TornadoRenderer.material;
    }
    private void Update()
    {
        if (currentState == AnimationState.None) return;

        elapsedTime += Time.deltaTime;

        switch (currentState)
        {
            case AnimationState.GrowingUp:
                AnimateTornado(TargetDuration, startSize, targetSize, startSpeed, targetSpeed, AnimationState.Waiting);
                if (elapsedTime >= TargetDuration)
                {
                    currentState = AnimationState.Waiting;
                    elapsedTime = 0f;
                }
                break;
            case AnimationState.Waiting:
                if (elapsedTime >= TornadoWaitTime)
                {
                    BlazeFire.SetActive(false);
                    FireBody.SetActive(true);

                    currentState = AnimationState.GrowingDown;
                    elapsedTime = 0f;

                    startSize = transform.localScale;
                    targetSize = Vector3.zero;
                    startSpeed = material.GetFloat("_TextureScrollYSpeed");
                    targetSpeed = 0f;
                }
                break;
            case AnimationState.GrowingDown:
                AnimateTornado(TargetDuration, startSize, targetSize, startSpeed, targetSpeed, AnimationState.None);
                if (elapsedTime >= TargetDuration)
                {
                    TornadoRenderer.gameObject.SetActive(false);
                    currentState = AnimationState.None;
                }
                break;
        }
    }

    public void TornadoAnimation()
    {
        currentState = AnimationState.GrowingUp;
        elapsedTime = 0f;
        startSize = transform.localScale;
        targetSize = TargetSize;
        startSpeed = material.GetFloat("_TextureScrollYSpeed");
        targetSpeed = TargetSpeed;
    }
    private void AnimateTornado(float duration, Vector3 startScale, Vector3 endScale, float startSpd, float endSpd, AnimationState nextState)
    {
        float t = elapsedTime / duration;
        float easeOutT = joonyle99.Math.EaseOutQuad(t);

        transform.localScale = Vector3.Lerp(startScale, endScale, easeOutT);
        material.SetFloat("_TextureScrollYSpeed", Mathf.Lerp(startSpd, endSpd, easeOutT));
    }

    /*
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
    */
}
