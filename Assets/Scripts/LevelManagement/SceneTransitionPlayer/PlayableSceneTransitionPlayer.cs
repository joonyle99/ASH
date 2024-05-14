using UnityEngine;
using System.Collections;
using Utils;

/*
 * 이 클래스가 담당하는 것: 씬 전환/재시작 시 fade 등의 화면 전환 효과 
 *  - Exit 또는 재시작은 누군가 호출해줘야하고, Enter은 씬 로드 시 자동호출 됨
 *  - 전환 시 플레이어 위치 변경 정도는 할 수 있으나, 기능적인 동작은 하지 않음 
 */
public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] protected float TransitionDuration = 0.5f;

    [Header("Respawn")]
    [SerializeField] float _respawnFadeDuration = 0.5f;
    [SerializeField] float _respawnDelay = 0.5f;
    [SerializeField] float _capeFlyDuration = 1f;
    public override IEnumerator ExitEffectCoroutine()
    {
        SceneEffectManager.Current.Camera.DisableCameraFollow();

        yield return FadeCoroutine(TransitionDuration, FadeType.Darken);
    }

    public override IEnumerator EnterEffectCoroutine()
    {
        StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Lighten));
        Passage entrance = SceneContext.Current.EntrancePassage;
        if (entrance == null)
        {
            yield break;
        }
        SceneEffectManager.Current.Camera.SnapFollow();

        yield return StartCoroutine(entrance.PlayerExitCoroutine());
    }

    private IEnumerator InstantRespawnEffectCoroutine(Vector3 spawnPosition)
    {
        var player = SceneContext.Current.Player;

        //yield return FadeCoroutine(_respawnFadeDuration, FadeType.Darken);

        yield return new WaitForSeconds(_respawnDelay);

        float eTime = 0f;
        var originalPosition = SceneContext.Current.Player.transform.position;

        while (eTime < _capeFlyDuration)
        {
            yield return null;
            eTime += Time.deltaTime;
            SceneContext.Current.Player.transform.position = Vector3.Lerp(originalPosition, spawnPosition, Curves.EaseOut(eTime/_capeFlyDuration));
        }

        SceneContext.Current.Player.transform.position = spawnPosition;

        //yield return FadeCoroutine(_respawnFadeDuration, FadeType.Lighten);

        player.ChangeState<IdleState>();
    }

    // TODO : Global 플레이어 상태 관리 오브젝트로 옮겨야함
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(InstantRespawnEffectCoroutine(spawnPosition));
    }


}