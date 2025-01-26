using TMPro;
using UnityEngine;

/// <summary>
/// 상호작용 텍스트 가이드를 출력하기 위한 UI
///
/// 상호작용 마커 UI 경우의 수
///     1. 플레이어가 상호작용 가능한 오브젝트의 트리거 박스에 들어간 경우
///     2. 플레이어와 오브젝트가 상호작용 중인 경우
///     3. 플레이어와 오브젝트의 상호작용이 거리가 멀어져 끝난 경우
///     4. 플레이어가 상호작용을 끝마친 경우
/// </summary>
public class InteractionMarker : MonoBehaviour
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private InteractableObject _currentObject;

    private bool _isMarking;
    public bool IsMarking => _isMarking;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Update()
    {
        if (_isMarking)
        {
            OptionView optionView = GameUIManager.Instance.OptionView;

            // 2. 플레이어와 오브젝트가 상호작용 중인 경우 (feat. InteractionMarker.cs)
            if (_currentObject.IsInteracting || optionView.IsPause)
            {
                InvisibleMarker();
            }
            else
            {
                if (_currentObject.IsInteractable)
                {
                    VisibleMarker();
                }
            }
        }
    }

    public void LateUpdate()
    {
        //
        // 요약:
        //     상호작용 텍스트 가이드의 위치 설정은 Update()에서 현재 '상호작용 가능한 오브젝트가 정해진 후'에 계산한다
        //

        if (_isMarking)
        {
            UpdateMarkerPos(_currentObject.InteractionMarkerPoint);
        }
    }

    public void EnableMarker(InteractableObject interactionObject)
    {
        if (!interactionObject.IsInteractable) return;

        // Debug.Log("Enable Marker");

        SetInteractionMarkerKey();

        gameObject.SetActive(true);

        _isMarking = true;

        _currentObject = interactionObject;
    }
    public void DisableMarker()
    {
        // Debug.Log("Disable Marker");

        gameObject.SetActive(false);

        _isMarking = false;

        _currentObject = null;
    }

    public void VisibleMarker()
    {
        if (_canvasGroup.alpha > 0.99f) return;
        
        // Debug.Log("Visible Marker");

        _canvasGroup.alpha = 1f;
    }

    public void InvisibleMarker()
    {
        if (_canvasGroup.alpha < 0.01f) return;

        // Debug.Log("Invisible Marker");

        _canvasGroup.alpha = 0f;
    }

    public void UpdateMarkerPos(Vector3 markerPosAtWorlds)
    {
        var markerPosAtScreen = Camera.main.WorldToScreenPoint(markerPosAtWorlds);
        _rectTransform.position = markerPosAtScreen;
    }

    public void SetInteractionMarkerKey()
    {
        TMP_Text keyText = transform.Find("Key Box").GetComponentInChildren<TMP_Text>();
        PCInputSetter pcInputSetter = InputManager.Instance.DefaultInputSetter as PCInputSetter;

        keyText.text = pcInputSetter.GetKeyCode("상호작용").KeyCode.ToString();
    }
}