using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum CurvePathType
{
    None,
    ZoomInBook,
    ZoomInBookRightPage,
}

[RequireComponent(typeof(Volume))]
public class NewPrologueManager : MonoBehaviour
{
    [SerializeField] private BezierCurvePath _zoomInBookPath;
    [SerializeField] private BezierCurvePath _zoomInBookRightPath;
    [SerializeField] private BezierCurvePath _zoomOutBookPath;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _vignetteIntensity;

    [Header("Book")]
    [SerializeField] private Book _book;
    [SerializeField] private AutoFlip _bookAutoFlip;
    [SerializeField] private float _flipDelayTime;
    [SerializeField] private float _flipTime;

    private void Start()
    {
        StartCoroutine(StartPrologueLogic());
    }

    public void Update()
    {
        // CHEAT: F12 키를 누르면 프롤로그 스킵
        if (Input.GetKeyDown(KeyCode.F12) && GameSceneManager.Instance.CheatMode == true)
        {
            StopAllCoroutines();
            StartCoroutine(StartGameLogic());
        }
    }

    private IEnumerator StartPrologueLogic()
    {
        yield return VignetteEffectLogic(3f, _vignetteIntensity);

        yield return MoveZoomInBookCameraLogic(2f, _zoomInBookPath);
        yield return new WaitForSeconds(3f);

        yield return FlipBookLogic(1);
        yield return new WaitForSeconds(2f);

        yield return MoveZoomInBookCameraLogic(2f, _zoomInBookRightPath);
        yield return new WaitForSeconds(3f);

        yield return FlipBookLogic(2);
        yield return new WaitForSeconds(2f);

        yield return MoveZoomInBookCameraLogic(2, _zoomOutBookPath, 7f);
        yield return new WaitForSeconds(2f);

        yield return StartGameLogic();
    }

    private IEnumerator StartGameLogic()
    {
        PersistentDataManager.ClearPersistentData();
        PersistentDataManager.ClearSavedPersistentData();

        yield return FadeOutLogic(5f);

        //yield return SceneContext.Current.SceneTransitionPlayer.ExitSceneEffectCoroutine();
        SceneChangeManager.Instance.ChangeToPlayableScene("1-1", "Enter 1-1");
    }

    private IEnumerator FadeOutLogic(float duration)
    {
        float timer = 0f;

        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;

            float alpha = Mathf.Lerp(0, 1, timer / duration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
        }
    }

    #region Camera Moving
    public void MoveCameraWithVezierCurvePath(int curvePathType)
    {
        CurvePathType pathType = (CurvePathType)curvePathType;
        BezierCurvePath curvePath = null;
        switch(pathType)
        {
            case CurvePathType.ZoomInBookRightPage:
                curvePath = _zoomInBookRightPath;
                break;
            case CurvePathType.ZoomInBook:
                curvePath = _zoomInBookPath;
                break;
                
        }

        StartCoroutine(MoveZoomInBookCameraLogic(2f, curvePath));
    }

    private IEnumerator MoveZoomInBookCameraLogic(float duration, BezierCurvePath curvePath, float rotateRoll = 0f)
    {
        float timer = 0f;
        if (curvePath == null) 
            yield break;
        
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;

            //위치 값
            float value = Mathf.Lerp(0f, 1f, timer /  duration);
            Vector3 cameraPos = curvePath.CalculateBezierPoint(value);
            SceneContext.Current.CameraController.MoveTo(cameraPos);

            //회전값
            value = Mathf.Lerp(0, rotateRoll, timer / duration);
            Vector3 camRot = SceneContext.Current.CameraController.transform.rotation.eulerAngles;
            camRot.z = value;
            SceneContext.Current.CameraController.transform.rotation = Quaternion.Euler(camRot);
        }
    }
    #endregion

    #region PostProcessing
    public void ApplyVignetteEffect()
    {
        StartCoroutine(VignetteEffectLogic(3f, 0.5f));
    }

    private IEnumerator VignetteEffectLogic(float duration, float maxIntensity)
    {
        float timer = 0f;
        Volume volume = GetComponent<Volume>();

        if (volume.profile.TryGet(out Vignette vignette))
        {
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                yield return null;

                vignette.intensity.value = Mathf.Lerp(0, maxIntensity, timer / duration);
            }
        }
    }
    #endregion

    #region Book
    public void FlipBook(int flipCount)
    {
        StartCoroutine(FlipBookLogic(flipCount));
    }

    private IEnumerator FlipBookLogic(int flipCount)
    {
        int endPage = _book.currentPage + flipCount * 2 - 1;
        if(endPage > _book.TotalPageCount)
        {
            endPage = _book.TotalPageCount;
        }

        _bookAutoFlip.PageFlipTime = _flipTime;

        while (true) {
            if (_book.currentPage > endPage)
                break;

            yield return _bookAutoFlip.FlipRightPage();

            yield return new WaitForSeconds(_flipDelayTime);
        }
    }

    #endregion
}
