using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionController : MonoBehaviour
{
    public virtual IEnumerator ExitEffectCoroutine()
    {
        yield return null;
    }

    public virtual IEnumerator EnterEffectCoroutine()
    {
        yield return null;
    }
}