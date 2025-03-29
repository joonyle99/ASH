using UnityEngine;
using System.Collections;
using static SceneTransitionPlayer;
using UnityEngine.Timeline;
using TMPro;

public class FireBossManager : MonoBehaviour
{
    [Header("[Tornado Effect]")]
    [SerializeField] private Tornado _tornado;

    [Space]

    [Header("[Environment]")]
    [SerializeField] private GameObject _invisibleWall_Left;
    [SerializeField] private GameObject _invisibleWall_Right;
    [SerializeField] private GameObject _invisibleWall_Left_2;
    [SerializeField] private GameObject _invisibleWall_Right_2;
    [SerializeField] private GameObject _footholds;

    [Space]

    [Header("[Rage Effect]")]
    [SerializeField] private ParticleHelper _rageEffectEmitting;
    [SerializeField] private ParticleHelper _rageEffectStaying;
    [SerializeField] private float _emittingWaitTime = 3f;
    [SerializeField] private float _emittingFadeTime = 2f;
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _fadeWaitTime = 2f;

    [Space]

    [Header("[Teleport Effect]")]
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private ParticleHelper _warpEffect;
    [SerializeField] private Transform _warpPoint;
    [SerializeField] private Vector2 _targetScale;
    [SerializeField] private float _targetDuration = 0.3f;

    [Space]

    [Header("[Camera]")]
    [SerializeField] private ChasingCamera _chasingCamera;

    [Space]

    [Header("[Ending]")]
    [SerializeField] private DialogueData _endingDialogue;
    [SerializeField] private Quest _endingQuest;
    [SerializeField] private Lantern _lastLantern;

    [Space]
    [SerializeField] private DialogueData _endingAceeptDialogue01;
    [SerializeField] private DialogueData _endingAceeptDialogue02;
    [SerializeField] private DialogueData _endingAceeptDialogue03;
    [SerializeField] private Transform _endingFireSpawnPoint;

    [Space]
    [SerializeField] private DialogueData _endingRejectDialogue;
    [SerializeField] private Quest _endingRejectQuest;
    [SerializeField] private DialogueData _endingRejectDialogue01;

    public enum EndingType
    {
        None,
        Accept,
        Reject
    }
    private EndingType _endingType = EndingType.None;
    public enum EndingRejectType
    {
        None,
        Reject01,
        Reject02
    }
    private EndingRejectType _endingRejectType = EndingRejectType.None;

    [Space]

    [Header("[Etc]")]
    [SerializeField] private TutorialZone _lightingGuide;
    [SerializeField] private SoundList _soundList;

    // fire find cutscene
    public void ExecuteTornadoEffect()
    {
        _tornado.gameObject.SetActive(true);
        _tornado.TornadoAnimation();
    }
    public void SetUpBattle()
    {
        StartCoroutine(SetUpBattleCoroutine());
    }
    private IEnumerator SetUpBattleCoroutine()
    {
        // set boundaries
        _invisibleWall_Left.SetActive(true);
        _invisibleWall_Right.SetActive(true);
        SetCameraBoundaries();

        // set camera size
        SceneContext.Current.CameraController.UpdateScreenSize(13f, 2f);

        yield return new WaitUntil(() => SceneContext.Current.CameraController.IsUpdateFinished);

        yield return null;
    }
    private void SetCameraBoundaries()
    {
        // 처음에 게임 오브젝트가 비활성화되어 있으면 Bounds가 유효하지 않기 때문에 여기서 가져온다
        var leftWallCollider = _invisibleWall_Left.GetComponent<BoxCollider2D>();
        var rightWallCollider = _invisibleWall_Right.GetComponent<BoxCollider2D>();

        if (leftWallCollider == null || rightWallCollider == null)
        {
            Debug.LogError("Left or Right Wall Collider is null");
            return;
        }

        var leftValue = _invisibleWall_Left.transform.position.x + leftWallCollider.bounds.extents.x;
        var rightValue = _invisibleWall_Right.transform.position.x - rightWallCollider.bounds.extents.x;

        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Left, true, leftValue);
        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Right, true, rightValue);
    }

    // fire rage cutscene
    public void ExecuteRageEffect()
    {
        StartCoroutine(ExecuteRageEffectCoroutine());
    }
    private IEnumerator ExecuteRageEffectCoroutine()
    {
        yield return ExecuteRageEffectCoroutine01();
        yield return ExecuteRageEffectCoroutine02();
    }
    private IEnumerator ExecuteRageEffectCoroutine01()
    {
        // playing
        _rageEffectEmitting.gameObject.SetActive(true);

        _soundList.PlaySFX("SE_Fire_Ashes");

        yield return new WaitForSeconds(_emittingWaitTime);

        /*
        var particle = _rageEffectEmitting.ParticleSystem;

        var burst = particle.emission.GetBurst(0);
        var burstDuration = (burst.cycleCount - 1) * burst.repeatInterval;      // cycle: 7, repeatInterval: 0.4 = 2.8
        yield return new WaitForSeconds(burstDuration);
        yield return new WaitForSeconds(_lastEmittingStopTime);

        particle.Pause();
        yield return new WaitForSeconds(_lastEmittingWaitTime);
        */

        // screen fade out => 2초
        yield return SceneContext.Current.SceneTransitionPlayer.FadeCoroutine(_fadeDuration, FadeType.Darken);

        // waiting => 2초
        yield return new WaitForSeconds(_fadeWaitTime);
        SceneEffectManager.Instance.Camera.StopConstantShake();
        _rageEffectEmitting.gameObject.SetActive(false);
        _rageEffectStaying.gameObject.SetActive(true);
    }
    private IEnumerator ExecuteRageEffectCoroutine02()
    {
        // screen fade in => 2초
        yield return SceneContext.Current.SceneTransitionPlayer.FadeCoroutine(_fadeDuration, FadeType.Dim);

        // effect fade out => 2초
        var particleRenderer = _rageEffectStaying.ParticleSystem.GetComponent<ParticleSystemRenderer>();
        var particleMaterial = particleRenderer.material;

        var eTime = 0f;
        var startAplpha = particleRenderer.material.GetFloat("_Alpha");

        while (eTime < _emittingFadeTime)
        {
            var t = eTime / _emittingFadeTime;

            var nextAlpha = Mathf.Lerp(startAplpha, 0f, t);
            particleMaterial.SetFloat("_Alpha", nextAlpha);

            yield return null;

            eTime += Time.deltaTime;
        }

        _rageEffectStaying.gameObject.SetActive(false);

        particleMaterial.SetFloat("_Alpha", startAplpha);
    }
    public void ExecuteTeleportEffect()
    {
        StartCoroutine(ExecuteTeleportEffectCoroutine());
    }
    private IEnumerator ExecuteTeleportEffectCoroutine()
    {
        var effectObject = _warpEffect.gameObject;
        effectObject.SetActive(true);

        var fireTransform = _firePrefab.transform;
        var startScale = fireTransform.localScale;

        var eTime = 0f;

        while (eTime < _targetDuration)
        {
            var t = eTime / _targetDuration;
            var easeTime = joonyle99.Math.EaseOutQuart(t);

            //fireTransform.localScale = new Vector3(
            //    Mathf.Lerp(startScale.x, _targetScale.x, easeTime),
            //    Mathf.Lerp(startScale.y, _targetScale.y, easeTime),
            //    startScale.z);

            fireTransform.localScale = Vector3.Lerp(startScale, _targetScale, easeTime);
            yield return null;
            eTime += Time.deltaTime;
        }

        fireTransform.position = _warpPoint.position;
        // **자식 Rigidbody 동기화**
        var fireRigidbodys = _firePrefab.GetComponentsInChildren<Rigidbody2D>();
        foreach (var fireRigidbody in fireRigidbodys)
        {
            fireRigidbody.position = _warpPoint.position;   // 위치 강제 동기화
            fireRigidbody.velocity = Vector2.zero;          // 속도 초기화 (이전 움직임 제거)
            fireRigidbody.angularVelocity = 0f;             // 회전 속도 초기화
        }

        fireTransform.localScale = startScale;                      // 원래 크기로 되돌림
        effectObject.SetActive(false);
    }
    public void ExecuteVisibleFootholds()
    {
        //_footholds.SetActive(true);
        StartCoroutine(ExecuteVisibleFootholdsCoroutine());
    }
    private IEnumerator ExecuteVisibleFootholdsCoroutine()
    {
        _footholds.SetActive(true);

        var spriteRenderers = _footholds.GetComponentsInChildren<SpriteRenderer>();

        var eTime = 0f;
        var duration = 2f;

        while (eTime < duration)
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                var t = eTime / duration;

                var nextAlpha = Mathf.Lerp(0f, 1f, t);

                var color = spriteRenderer.color;
                color.a = nextAlpha;
                spriteRenderer.color = color;
            }

            yield return null;

            eTime += Time.deltaTime;
        }
    }

    // extension fire rage cutscene
    public IEnumerator SetUpRageCoroutine()
    {
        // set boundaries
        _invisibleWall_Left.SetActive(false);
        _invisibleWall_Right.SetActive(false);
        _invisibleWall_Left_2.SetActive(true);
        _invisibleWall_Right_2.SetActive(true);
        SetCameraBoundaries2();

        // set camera size
        SceneContext.Current.CameraController.UpdateScreenSize(15f, 2f);

        yield return new WaitUntil(() => SceneContext.Current.CameraController.IsUpdateFinished);

        yield return null;

        //Debug.Log("SetUpRageCoroutine 끝");
    }
    private void SetCameraBoundaries2()
    {
        // 처음에 게임 오브젝트가 비활성화되어 있으면 Bounds가 유효하지 않기 때문에 여기서 가져온다
        var leftWallCollider = _invisibleWall_Left_2.GetComponent<BoxCollider2D>();
        var rightWallCollider = _invisibleWall_Right_2.GetComponent<BoxCollider2D>();

        if (leftWallCollider == null || rightWallCollider == null)
        {
            Debug.LogError("Left or Right Wall Collider is null");
            return;
        }

        var leftValue = _invisibleWall_Left_2.transform.position.x + leftWallCollider.bounds.extents.x;
        var rightValue = _invisibleWall_Right_2.transform.position.x - rightWallCollider.bounds.extents.x;

        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Left, true, leftValue);
        SceneContext.Current.CameraController.SetBoundaries(CameraController.BoundaryType.Right, true, rightValue);
    }

    public void ActiveCameraChasing()
    {
        _chasingCamera.enabled = true;          // chasing camera의 OnEnable에서 event listener를 등록하는 로직이 실행된다
        _chasingCamera.StartChasing();
    }
    public void DeActiveCameraChasing()
    {
        _chasingCamera.StopChasing();
        _chasingCamera.enabled = false;         // chasing camera의 OnDisable에서 event listener를 해제하는 로직이 실행된다
    }
    public void StartCameraChasing()
    {
        _chasingCamera.StartChasing();
    }
    public void StopCameraChasing()
    {
        _chasingCamera.StopChasing();
    }

    public IEnumerator StartEndingDialogue()
    {
        _endingDialogue.LinkQuestData(_endingQuest);

        while (true)
        {
            DialogueController.Instance.StartDialogue(_endingDialogue, false);
            yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

            // Accept Process
            if (_endingType == EndingType.Accept)
            {
                
                var fire = _firePrefab.GetComponentInChildren<Fire>();
                var ash = SceneContext.Current.Player;

                yield return PlayerMoveCoroutine();
                yield return LightSkillCoroutine();

                yield return ExecuteRageEffectCoroutine01();

                SceneEffectManager.Instance.Camera.StartFollow(ash.transform);
                _tornado.FireBody.SetActive(false);
                _tornado.BlazeFire.SetActive(true);
                fire.transform.position = _endingFireSpawnPoint.position;
                fire.RigidBody2D.position = _endingFireSpawnPoint.position;

                yield return ExecuteRageEffectCoroutine02();

                DialogueController.Instance.StartDialogue(_endingAceeptDialogue01, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                //StartCoroutine(fire.DeathEffectCoroutine());
                //yield return new WaitForSeconds(1.5f);
                //yield return _tornado.FadeOutBlaze();
                _tornado.BlazeFire.SetActive(false);

                DialogueController.Instance.StartDialogue(_endingAceeptDialogue02, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                yield return ash.DeathEffectCoroutine();

                DialogueController.Instance.StartDialogue(_endingAceeptDialogue03, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
                SceneChangeManager.Instance.ChangeToNonPlayableScene("EndingScene_Peace");
            }
            // Reject Process
            else if (_endingType == EndingType.Reject)
            {
                // 난 너의 선택을 존중해
                // 하지만 여기서 그만둔다면 바뀌는 건 없을 거야
                _endingRejectDialogue.LinkQuestData(_endingRejectQuest);
                DialogueController.Instance.StartDialogue(_endingRejectDialogue, false);
                yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                if (_endingRejectType == EndingRejectType.Reject01)
                {
                    // 지금이라도 늦지 않았다.
                    // 그대의 목숨과 맞바꾸어 이 세상을 구할것인가?
                }
                else if (_endingRejectType == EndingRejectType.Reject02)
                {
                    // 크윽... 죽음 앞에선 너도 별 수 없구나
                    DialogueController.Instance.StartDialogue(_endingRejectDialogue01, false);
                    yield return new WaitUntil(() => DialogueController.Instance.IsDialoguePanel == false);

                    GameUIManager.OpenDeadPanel(true);

                    yield break;
                }
                else
                {
                    Debug.LogError("EndingRejectType is invalid");
                    yield break;
                }
            }
            else
            {
                Debug.LogError("EndingType is invalid");
                yield break;
            }
        }
    }
    public void AcceptCallback()
    {
        Debug.Log("Accept");
        _endingType = EndingType.Accept;
    }
    public void RejectCallback()
    {
        Debug.Log("Reject");
        _endingType = EndingType.Reject;
    }
    public void Reject01Callback()
    {
        Debug.Log("Reject01");
        _endingRejectType = EndingRejectType.Reject01;
    }
    public void Reject02Callback()
    {
        Debug.Log("Reject02");
        _endingRejectType = EndingRejectType.Reject02;
    }
    private IEnumerator PlayerMoveCoroutine()
    {
        yield return new WaitForSeconds(1f);

        // 플레이어 위치
        var player = SceneContext.Current.Player;
        var playerPosX = player.transform.position.x;
        Debug.DrawRay(player.transform.position, Vector3.down * 5f, Color.cyan, 10f);

        // 마지막 랜턴에서 플레이어까지의 방향
        var lanternToPlayerDir = System.Math.Sign(playerPosX - _lastLantern.transform.position.x);
        var _distanceFromLantern = 4f;
        Debug.DrawRay(_lastLantern.transform.position + Vector3.up, Vector3.right * lanternToPlayerDir * _distanceFromLantern, Color.cyan, 10f);

        // 플레이어가 이동할 목표 위치
        var playerMoveTargetPosX = _lastLantern.transform.position.x + (lanternToPlayerDir) * _distanceFromLantern;
        Debug.DrawRay(new Vector3(playerMoveTargetPosX, _lastLantern.transform.position.y, _lastLantern.transform.position.z), Vector3.down * 5f, Color.cyan, 10f);

        // 플레이어 이동 방향
        var playerMoveDir = System.Math.Sign(playerMoveTargetPosX - playerPosX);
        Debug.DrawRay(player.transform.position, Vector3.right * playerMoveDir * 5f, Color.cyan, 10f);

        // 플레이어 이동을 대기
        yield return MoveCoroutine(playerMoveDir, playerMoveTargetPosX);

        // 만약 플레이어가 뒤돌고 있다면 방향을 돌려준다
        if (lanternToPlayerDir == player.RecentDir)
        {
            var dirForLookToLantern = (-1) * playerMoveDir;
            yield return MoveCoroutine(dirForLookToLantern, playerMoveTargetPosX + dirForLookToLantern * 0.2f);
        }

        InputManager.Instance.ChangeToStayStillSetter();
    }
    private IEnumerator MoveCoroutine(int moveDir, float targetPosX)
    {
        var isRight = moveDir > 0;

        if (isRight)
        {
            InputManager.Instance.ChangeToMoveRightSetter();
        }
        else
        {
            InputManager.Instance.ChangeToMoveLeftSetter();
        }

        yield return new WaitUntil(() => System.Math.Abs(targetPosX - SceneContext.Current.Player.transform.position.x) < 0.2f);
    }
    private IEnumerator LightSkillCoroutine()
    {
        InputManager.Instance.ChangeToOnlyLightSetter();

        var startTime = Time.time;
        yield return new WaitUntil(() => _lastLantern.IsLightOn || Time.time - startTime > 5f);
        if (!_lastLantern.IsLightOn) { ToggleLightingGuide(); }
        yield return new WaitUntil(() => _lastLantern.IsLightOn); // 5초 이상 지속된 경우에만 빛 가이드를 통해서 들어오겠지

        yield return new WaitUntil(() => LanternSceneContext.Current.IsEndLastAttack == true);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    ChangeSceneToEndingPeaceful();
        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    ToggleLightingGuide();
        //}

        if (_lightingGuide.gameObject.activeSelf)
        {
            _lightingGuide.GetComponent<RectTransform>().position = SceneContext.Current.Player.transform.position + Vector3.up * 3f;
        }
    }
    public void ChangeSceneToEndingPeaceful()
    {
        SceneEffectManager.StopPlayingCutscene();

        SceneChangeManager.Instance.ChangeToNonPlayableScene("EndingScene_Peace", () =>
        {
            EndingSceneManager.Initialize();
            EndingSceneManager.PlayCutscene("EndingCutscene_SurroundingScene");
        });
    }
    public void ToggleLightingGuide()
    {
        var isActive = _lightingGuide.gameObject.activeSelf;
        if (!isActive)
        {
            _lightingGuide.StopAllCoroutines();
        }
        _lightingGuide.gameObject.SetActive(!isActive);
    }
}
