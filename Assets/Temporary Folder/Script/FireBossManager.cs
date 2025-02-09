using UnityEngine;
using System.Collections;
using static SceneTransitionPlayer;
using System.Collections.Generic;

public class FireBossManager : MonoBehaviour
{
    // tornado effect
    [SerializeField] private Tornado _tornado;

    [Space]

    // boundaries
    [SerializeField] private GameObject _invisibleWall_Left;
    [SerializeField] private GameObject _invisibleWall_Right;

    [Space]

    // rage effect
    [SerializeField] private ParticleHelper _rageEffectEmitting;
    [SerializeField] private ParticleHelper _rageEffectStaying;
    [SerializeField] private float _emittingWaitTime = 3f;
    [SerializeField] private float _emittingFadeTime = 2f;
    [SerializeField] private float _fadeDuration = 2f;
    [SerializeField] private float _fadeWaitTime = 2f;

    [Space]

    [SerializeField] private GameObject _footholds;

    [Space]

    // teleport effect
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private ParticleHelper _warpEffect;
    [SerializeField] private Transform _warpPoint;
    [SerializeField] private Vector2 _targetScale;
    [SerializeField] private float _targetDuration = 0.3f;

    [Space]

    // camera chasing
    [SerializeField] private ChasingCamera _chasingCamera;

    [Space]

    // 마지막 대화
    [SerializeField] private DialogueData _dialogueData;

    [SerializeField] private SoundList _soundList;

    // fire find cutscene
    public void ExecuteTornadoEffect()
    {
        _tornado.gameObject.SetActive(true);
        _tornado.TornadoAnimation();
    }
    public void SetCameraBoundaries()
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

    // fire rage cutscene
    public void ExecuteRageEffect()
    {
        StartCoroutine(ExecuteRageEffectCoroutine());
    }
    private IEnumerator ExecuteRageEffectCoroutine()
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

            fireTransform.localScale = new Vector3(
                Mathf.Lerp(startScale.x, _targetScale.x, easeTime),
                Mathf.Lerp(startScale.y, _targetScale.y, easeTime),
                startScale.z);

            yield return null;

            eTime += Time.deltaTime;
        }

        // fireTransform.GetChild(0).position = _warpPoint.position;
        // _fireRigidbody.position = _warpPoint.position;

        // TODO: 이상하게 동작한다

        fireTransform.position = _warpPoint.position;               // 텔레포트 이동
        fireTransform.localScale = startScale;                      // 원래 크기로 되돌림

        effectObject.SetActive(false);
    }
    public void ExecuteVisibleFootholds()
    {
        _footholds.SetActive(true);
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

    public IEnumerator StartLastDialogue()
    {
        // 이렇게 하면 그냥 대화창을 넘겨버리면 .. 안되니까,,

        // 아쉽게도 마지막 대화창을 유지시키지는 못할 것 같다.
        DialogueController.Instance.StartDialogue(_dialogueData, false);

        // 응답 패널 열기
        // 이 응답 패널은 좀 새로운 형태로 만들어도 될 거 같기도..?
        var view = DialogueController.Instance.View;

        List<ResponseContainer> responseFunctions = new List<ResponseContainer>();
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Accept, Accept));
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Reject, Reject));
        view.OpenResponsePanel(responseFunctions);

        // Handler: 이벤트가 발생했을 때 호출되는 함수를 지칭한다 (옵저버 패턴)
        var isClicked = false;
        void ResponseHandler()
        {
            isClicked = true;
            view.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
            view.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
        }
        view.ResponsePanel.Accept.onClick.RemoveListener(ResponseHandler);
        view.ResponsePanel.Accept.onClick.AddListener(ResponseHandler);
        view.ResponsePanel.Reject.onClick.RemoveListener(ResponseHandler);
        view.ResponsePanel.Reject.onClick.AddListener(ResponseHandler);

        // 해당 퀘스트가 수락 / 거절되기 전까지 대기
        yield return new WaitUntil(() => isClicked);

        // 퀘스트 응답 종료 사운드 재생
        SoundManager.Instance.PlayCommonSFX("SE_UI_Select");
    }
    private void Accept()
    {
        Debug.Log("Accept");
        DialogueController.Instance.ShutdownDialogue();
    }
    private void Reject()
    {
        Debug.Log("Reject");
        DialogueController.Instance.ShutdownDialogue();
    }
}
