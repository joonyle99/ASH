using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] float _transitionDuration = 0.5f;
    public string EntrancePassageName { get; set; }
    public override IEnumerator ExitEffectCoroutine()
    {
        Camera.main.GetComponent<CameraController>().DisableCameraFollow();

        yield return FadeCoroutine(_transitionDuration, FadeType.Darken);
        yield return null;
    }

    public override IEnumerator EnterEffectCoroutine()
    {
        StartCoroutine(FadeCoroutine(_transitionDuration, FadeType.Darken));
        Passage entrance = SceneContext.Current.Passages.Find(x => x.PassageName == EntrancePassageName);
        if (entrance == null)
        {
            Debug.LogWarning("Passage " + EntrancePassageName + " is not found in this scene !!");
            if (SceneContext.Current.Passages.Count > 0)
                entrance = SceneContext.Current.Passages[0];
            else
                yield break;
        }
        Camera.main.GetComponent<CameraController>().SnapFollow();
        yield return StartCoroutine(entrance.PlayerExitCoroutine());

        yield return null;

    }



}