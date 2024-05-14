using UnityEngine;
using System.Collections;
using Utils;

/*
 * �� Ŭ������ ����ϴ� ��: �� ��ȯ/����� �� fade ���� ȭ�� ��ȯ ȿ�� 
 *  - Exit �Ǵ� ������� ������ ȣ��������ϰ�, Enter�� �� �ε� �� �ڵ�ȣ�� ��
 *  - ��ȯ �� �÷��̾� ��ġ ���� ������ �� �� ������, ������� ������ ���� ���� 
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

    // TODO : Global �÷��̾� ���� ���� ������Ʈ�� �Űܾ���
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(InstantRespawnEffectCoroutine(spawnPosition));
    }


}