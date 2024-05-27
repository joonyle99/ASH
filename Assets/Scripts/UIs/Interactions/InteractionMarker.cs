using UnityEngine;

/// <summary>
/// 상호작용 텍스트 가이드를 출력하기 위한 UI
/// </summary>
public class InteractionMarker : MonoBehaviour
{
    private RectTransform _rectTransform;
    private InteractableObject _currentObject;

    private bool _isMarking;
    public bool IsMarking => _isMarking;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void LateUpdate()
    {
        //
        // 요약:
        //     상호작용 텍스트 가이드의 위치 설정은 Update()에서 현재 '상호작용 가능한 오브젝트가 정해진 후'에 계산한다
        //

        if (_isMarking)
            UpdateMarkerPos(_currentObject.InteractionMarkerPoint);
    }

    public void EnableMarkerAt(InteractableObject interactionObject)
    {
        gameObject.SetActive(true);                     // UI 게임 오브젝트 활성화
        _isMarking = true;

        _currentObject = interactionObject;             // 현재 상호작용 중인 오브젝트 업데이트

        UpdateMarkerPos(interactionObject.InteractionMarkerPoint);
    }
    public void DisableMarker()
    {
        _isMarking = false;
        gameObject.SetActive(false);
    }
    public void UpdateMarkerPos(Vector3 markerPosAtWorlds)
    {
        var markerPosAtScreen = Camera.main.WorldToScreenPoint(markerPosAtWorlds);
        _rectTransform.position = markerPosAtScreen;

        // var centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f);
        // _rectTransform.anchoredPosition = markerPosAtScreen - centerOfScreen;
    }
}