using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

/*
 * �� Ŭ������ ����ϴ� ��: �� ��ȯ/����� �� fade ���� ȭ�� ��ȯ ȿ�� 
 *  - Exit �Ǵ� ������� ������ ȣ��������ϰ�, Enter�� �� �ε� �� �ڵ�ȣ�� ��
 *  - ��ȯ �� �÷��̾� ��ġ ���� ������ �� �� ������, ������� ������ ���� ���� 
 */
public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] float _transitionDuration = 0.5f;

    [Header("Respawn")]
    //[SerializeField] float _respawnFadeDuration = 0.5f;
    [SerializeField] float _respawnDelay = 0.5f;
    [SerializeField] float _capeFlyDuration = 1f;
    public override IEnumerator ExitEffectCoroutine()
    {
        CameraControlToken token = new CameraControlToken(CameraPriority.SceneChange);
        yield return new WaitUntil(() => token.IsAvailable);
        token.Camera?.DisableCameraFollow();
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
        token.Camera?.SnapFollow();
        token.Release();

        yield return StartCoroutine(entrance.PlayerExitCoroutine());
    }

    IEnumerator InstantRespawnEffectCoroutine(Vector3 spawnPosition)
    {
        var respawnState = SceneContext.Current.Player.GetComponent<InstantRespawnState>();
        yield return new WaitForSeconds(respawnState.DieDuration);
        //yield return FadeCoroutine(_respawnFadeDuration, FadeType.Darken);
        yield return new WaitForSeconds(_respawnDelay);

        float eTime = 0f;
        var originalPosition = SceneContext.Current.Player.transform.position;
        while (eTime < _capeFlyDuration)
        {
            SceneContext.Current.Player.transform.position = Vector3.Lerp(originalPosition, spawnPosition, Curves.EaseOut(eTime/_capeFlyDuration));
            yield return null;
            eTime += Time.deltaTime;
        }
        SceneContext.Current.Player.transform.position = spawnPosition;

        //yield return FadeCoroutine(_respawnFadeDuration, FadeType.Lighten);
        respawnState.Respawn();
    }

    // TODO : Global �÷��̾� ���� ���� ������Ʈ�� �Űܾ���
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(InstantRespawnEffectCoroutine(spawnPosition));
    }


}