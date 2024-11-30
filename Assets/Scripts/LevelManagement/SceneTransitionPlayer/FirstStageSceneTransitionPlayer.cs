using UnityEngine;
using System.Collections;

public class FirstStageSceneTransitionPlayer : PlayableSceneTransitionPlayer
{
    [Header("First enter cutscene")]
    [Space]

    [SerializeField] private  Transform _spawnPoint;
    [SerializeField] private  GameObject _spawnFireEffectOnScene;
    [SerializeField] private  float _waitDuration = 1f;
    [SerializeField] private  float _playerSpawnTiming = 0.3f;
    [SerializeField] private  float _delayAfterSpawn = 2f;

    public override IEnumerator EnterSceneEffectCoroutine()
    {
        // Debug.Log("call EnterSceneEffectCoroutine in FirstStageSceneTransitionPlayer");

        if (PersistentDataManager.GetByGlobal<bool>("seenPlayerFirstSpawn"))
        {
            yield return base.EnterSceneEffectCoroutine();
        }
        else
        {
            SceneContext.Current.Player.transform.position = _spawnPoint.position;
            SceneContext.Current.Player.gameObject.SetActive(false);
            // SceneEffectManager.Instance.Camera.SnapFollow();
            GameUIManager.OpenLetterbox(true);
            StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Lighten));

            InputManager.Instance.ChangeToStayStillSetter();

            yield return new WaitForSeconds(_waitDuration);
            _spawnFireEffectOnScene?.SetActive(true);
            yield return new WaitForSeconds(_playerSpawnTiming);

            SceneContext.Current.Player.gameObject.SetActive(true);
            yield return new WaitForSeconds(_delayAfterSpawn);

            GameUIManager.CloseLetterbox();
            InputManager.Instance.ChangeToDefaultSetter();
            PersistentDataManager.SetByGlobal("seenPlayerFirstSpawn", true);
        }
    }

}