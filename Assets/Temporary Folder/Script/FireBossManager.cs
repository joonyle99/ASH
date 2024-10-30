using UnityEngine;
using System.Collections;
using static SceneTransitionPlayer;

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
    [SerializeField] private AnimationCurve _scaleXCurve;
    [SerializeField] private AnimationCurve _scaleYCurve;
    [SerializeField] private Transform _warpPoint;
    [SerializeField] private float _duration;

    [Space]

    // camera chasing
    [SerializeField] private ChasingCamera _chasingCamera;

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

        var fireTrans = _firePrefab.transform;
        var startScale = fireTrans.localScale;

        var eTime = 0f;

        while (eTime < _duration)
        {
            var curveValueX = _scaleXCurve.Evaluate(eTime);
            var curveValueY = _scaleYCurve.Evaluate(eTime);

            var nextScale = new Vector3(curveValueX, curveValueY, startScale.z);

            fireTrans.localScale = nextScale;

            yield return null;

            eTime += Time.deltaTime;
        }

        fireTrans.position = _warpPoint.position;
        fireTrans.localScale = startScale;
        effectObject.SetActive(false);
    }
    public void ExecuteVisibleFootholds()
    {
        _footholds.SetActive(true);
    }

    public void StartCameraChasing()
    {
        _chasingCamera.enabled = true;
        _chasingCamera.StartChasing();
    }
    public void StopCameraChasing()
    {
        _chasingCamera.StopChasing();
        _chasingCamera.enabled = false;
    }
}
