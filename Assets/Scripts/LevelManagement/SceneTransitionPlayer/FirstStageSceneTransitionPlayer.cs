using UnityEngine;
using System.Collections;

/*
 * 이 클래스가 담당하는 것: 씬 전환/재시작 시 fade 등의 화면 전환 효과 
 *  - Exit 또는 재시작은 누군가 호출해줘야하고, Enter은 씬 로드 시 자동호출 됨
 *  - 전환 시 플레이어 위치 변경 정도는 할 수 있으나, 기능적인 동작은 하지 않음 
 */
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
        if (PersistentDataManager.Get<bool>("seenPlayerFirstSpawn"))
        {
            yield return base.EnterSceneEffectCoroutine();
        }
        else
        {
            SceneContext.Current.Player.transform.position = _spawnPoint.position;
            SceneContext.Current.Player.gameObject.SetActive(false);
            SceneEffectManager.Current.Camera.SnapFollow();
            GameUIManager.OpenLetterbox(true);
            StartCoroutine(FadeCoroutine(TransitionDuration, FadeType.Lighten));

            InputManager.Instance.ChangeToStayStillSetter();

            yield return new WaitForSeconds(_waitDuration);
            _spawnFireEffectOnScene.SetActive(true);
            yield return new WaitForSeconds(_playerSpawnTiming);

            SceneContext.Current.Player.gameObject.SetActive(true);
            yield return new WaitForSeconds(_delayAfterSpawn);

            GameUIManager.CloseLetterbox();
            InputManager.Instance.ChangeToDefaultSetter();
            PersistentDataManager.Set("seenPlayerFirstSpawn", true);
        }
    }

}