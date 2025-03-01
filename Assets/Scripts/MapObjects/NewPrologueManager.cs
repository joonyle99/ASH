using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class NewPrologueManager : MonoBehaviour
{
    [SerializeField, HideInInspector] private CameraController _cameraController;
    [SerializeField] private BezierCurvePath _bezierCurvePath;

    [SerializeField] private CutscenePlayer _cutscene;

    [Header("Book")]
    [SerializeField] private Book _book;
    [SerializeField] private AutoFlip _bookAutoFlip;
    [SerializeField] private float _flipDelayTime;
    [SerializeField] private float _flipTime;

    private void Start()
    {
        _cameraController = SceneContext.Current.CameraController;
        _cutscene.Play();
    }
    #region Camera Moving
    public void MoveCameraWithVezierCurvePath()
    {
        StartCoroutine(MoveCameraLogic(2f));
    }

    private IEnumerator MoveCameraLogic(float duration)
    {
        float timer = 0f;
        
        while(timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;

            float value = Mathf.Lerp(0f, 1f, timer /  duration);
            Vector3 cameraPos = _bezierCurvePath.CalculateBezierPoint(value);
            _cameraController.MoveTo(cameraPos);
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
        int endPage = flipCount * 2 - 1;
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
