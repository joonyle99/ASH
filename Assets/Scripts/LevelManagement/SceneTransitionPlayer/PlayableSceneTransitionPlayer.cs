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

    /// <summary>
    /// �� ���� �� ȭ���� ��� ����� ȿ��
    /// </summary>
    /// <returns></returns>
    public override IEnumerator EnterSceneEffectCoroutine()
    {
        IsPlayable = false;

        //Debug.Log($"Enter Scene Effect ����");

        //Debug.Log("call EnterSceneEffectCoroutine in PlayableSceneTransitionPlayer");

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Lighten));

        Passage entrance = SceneContext.Current.EntrancePassage;

        if (entrance == null) yield break;

        // SceneEffectManager.Instance.Camera.SnapFollow();

        yield return StartCoroutine(entrance.PlayerExitCoroutine());

        // Debug.Log($"Enter Scene Effect ����");

        // ** ���⼭ ���� �� ���� �� �÷��� �ؾ��ϴ� �ƾ��� �ִٸ� ���� **
        // yield return ���� �ʰ� ��� �����Ѵ�
        yield return StartCoroutine(entrance.PlayEnterCutscene());

        IsPlayable = true;
    }
    /// <summary>
    /// �� ���� �� ȭ���� ��Ӱ� ����� ȿ��
    /// </summary>
    /// <returns></returns>
    public override IEnumerator ExitSceneEffectCoroutine()
    {
        IsPlayable = false;

        SceneEffectManager.Instance.Camera.DisableCameraFollow();

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
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