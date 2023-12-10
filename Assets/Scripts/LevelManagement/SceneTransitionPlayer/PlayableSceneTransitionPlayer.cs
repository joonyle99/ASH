using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 이 클래스가 담당하는 것: 씬 전환/재시작 시 fade 등의 화면 전환 효과 
 *  - Exit 또는 재시작은 누군가 호출해줘야하고, Enter은 씬 로드 시 자동호출 됨
 *  - 전환 시 플레이어 위치 변경 정도는 할 수 있으나, 기능적인 동작은 하지 않음 
 */
public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] float _transitionDuration = 0.5f;

    [Header("Respawn")]
    [SerializeField] float _respawnFadeDuration = 0.5f;
    [SerializeField] float _respawnDelay = 0.5f;
    public override IEnumerator ExitEffectCoroutine()
    {
        CameraControlToken token = new CameraControlToken(CameraPriority.SceneChange);
        yield return new WaitUntil(() => token.IsAvailable);
        token.Camera.DisableCameraFollow();
        token.Release();

        yield return FadeCoroutine(_transitionDuration, FadeType.Darken);
    }

    public override IEnumerator EnterEffectCoroutine()
    {
        StartCoroutine(FadeCoroutine(_transitionDuration, FadeType.Darken));
        Passage entrance = SceneContext.Current.EntrancePassage;
        if (entrance == null)
        {
            yield break;
        }
        CameraControlToken token = new CameraControlToken(CameraPriority.SceneChange);
        yield return new WaitUntil(() => token.IsAvailable);
        token.Camera.SnapFollow();
        token.Release();

        yield return StartCoroutine(entrance.PlayerExitCoroutine());
    }

    IEnumerator RespawnEffectCoroutine(Vector3 spawnPosition)
    {
        yield return FadeCoroutine(_respawnFadeDuration, FadeType.Darken);
        SceneContext.Current.Player.gameObject.SetActive(false);
        yield return new WaitForSeconds(_respawnDelay);
        SceneContext.Current.Player.transform.position = spawnPosition;
        SceneContext.Current.Player.gameObject.SetActive(true);
        yield return FadeCoroutine(_respawnFadeDuration, FadeType.Lighten);
    }

    // TODO : Global 플레이어 상태 관리 오브젝트로 옮겨야함
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(RespawnEffectCoroutine(spawnPosition));
    }


}