using UnityEngine;
using System.Collections;
using Utils;

/// <summary>
/// �� Ŭ������ ����ϴ� ��: �� ��ȯ / ����� �� fade ���� ȭ�� ��ȯ ȿ��
/// - Exit �Ǵ� ������� ������ ȣ������� �ϰ�, Enter�� �� �ε� �� �ڵ�ȣ�� ��
/// - ��ȯ �� �÷��̾� ��ġ ���� ������ �� �� ������, ������� ������ ���� ����
/// </summary>
public class PlayableSceneTransitionPlayer : SceneTransitionPlayer
{
    [SerializeField] protected float TransitionDuration = 0.5f;

    [Header("Respawn")]
    [Space]

    [SerializeField] float _respawnDelay = 0.5f;
    [SerializeField] float _capeFlyDuration = 1f;

    public bool IsPlayable = true;

    public override IEnumerator EnterSceneEffectCoroutine()
    {
        IsPlayable = false;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Lighten));

        Passage entrance = SceneContext.Current.EntrancePassage;

        if (entrance == null) yield break;

        yield return entrance.PlayerExitCoroutine();

        // ** ���⼭ ���� �� ���� �� �÷��� �ؾ��ϴ� �ƾ��� �ִٸ� ���� **
        // yield return ���� �ʰ� ��� �����Ѵ�
        yield return entrance.PlayEnterCutscene();

        IsPlayable = true;
    }
    public override IEnumerator ExitSceneEffectCoroutine()
    {
        IsPlayable = false;

        SceneEffectManager.Instance.Camera.DisableCameraFollow();

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Darken));

        yield return _fadeCoroutine;

        IsPlayable = true;
    }

    // TODO : Global �÷��̾� ���� ���� ������Ʈ�� �Űܾ���
    public void PlayInstantRespawnEffect(Vector3 spawnPosition)
    {
        StartCoroutine(InstantRespawnEffectCoroutine(spawnPosition));
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
            SceneContext.Current.Player.transform.position = Vector3.Lerp(originalPosition, spawnPosition, Curves.EaseOut(eTime / _capeFlyDuration));
        }

        SceneContext.Current.Player.transform.position = spawnPosition;

        //yield return FadeCoroutine(_respawnFadeDuration, FadeType.Lighten);

        player.ChangeState<IdleState>();
    }
}