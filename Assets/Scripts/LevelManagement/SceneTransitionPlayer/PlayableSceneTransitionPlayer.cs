using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * �� Ŭ������ ����ϴ� ��: �� ��ȯ/����� �� fade ���� ȭ�� ��ȯ ȿ�� 
 *  - Exit �Ǵ� ������� ������ ȣ��������ϰ�, Enter�� �� �ε� �� �ڵ�ȣ�� ��
 *  - ��ȯ �� �÷��̾� ��ġ ���� ������ �� �� ������, ������� ������ ���� ���� 
 */
public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] float _transitionDuration = 0.5f;

    [Header("Respawn")]
    [SerializeField] float _respawnFadeDuration = 0.5f;
    [SerializeField] float _respawnDelay = 0.5f;
    public override IEnumerator ExitEffectCoroutine()
    {
        Camera.main.GetComponent<CameraController>().DisableCameraFollow();

        yield return FadeCoroutine(_transitionDuration, FadeType.Darken);
        yield return null;
    }

    public override IEnumerator EnterEffectCoroutine()
    {
        StartCoroutine(FadeCoroutine(_transitionDuration, FadeType.Darken));
        Passage entrance = SceneContext.Current.EntrancePassage;
        if (entrance == null)
        {
            yield break;
        }
        Camera.main.GetComponent<CameraController>().SnapFollow();
        yield return StartCoroutine(entrance.PlayerExitCoroutine());

        yield return null;
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

    // TODO : Global �÷��̾� ���� ���� ������Ʈ�� �Űܾ���
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(RespawnEffectCoroutine(spawnPosition));
    }


}