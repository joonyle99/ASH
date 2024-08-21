using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    private SoundList _soundList;

    private void Awake()
    {
        _soundList = GetComponent<SoundList>();
    }
    private void OnEnable()
    {
        _soundList.PlaySFX("Tornado");
    }
    private void OnDisable()
    {
        // _soundList.StopSFX("Tornado", isLoop: true);
    }

    public void TornadoAnimation()
    {
        StartCoroutine(TornadoAnimationCoroutine());
    }
    private IEnumerator TornadoAnimationCoroutine()
    {
        yield return GrowEffectCoroutine(3.5f);

        yield return ShrinkEffectCoroutine(3.5f);

        yield return null;

        gameObject.SetActive(false);
    }

    private IEnumerator GrowEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(2.3f, 2.3f, 2.3f);

        // speed

        // brightness

        while (eTime < duration)
        {
            var t = eTime / duration;

            // size
            transform.localScale = Vector3.Lerp(startSize, targetSize, t);

            // speed

            // brightness

            yield return null;
            eTime += Time.deltaTime;
        }
    }
    private IEnumerator ShrinkEffectCoroutine(float duration)
    {
        var eTime = 0f;

        // size
        var startSize = transform.localScale;
        var targetSize = new Vector3(0f, 0f, 0f);

        // speed

        // brightness

        while (eTime < duration)
        {
            var t = eTime / duration;

            // size
            transform.localScale = Vector3.Lerp(startSize, targetSize, t);

            // speed

            // brightness

            yield return null;
            eTime += Time.deltaTime;
        }
    }
}
