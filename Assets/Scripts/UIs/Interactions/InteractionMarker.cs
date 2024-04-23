using UnityEngine;

/// <summary>
/// 상호작용 텍스트 가이드를 출력하기 위한 UI
/// </summary>
public class InteractionMarker : MonoBehaviour
{
    private RectTransform _rectTransform;
    private InteractableObject _currentInteractable;

    private bool _isMarking;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    /// <summary>
    /// 상호작용 텍스트 가이드의 위치 설정은 Update()에서 현재 상호작용 가능한 오브젝트가 정해진 후에 계산한다
    /// </summary>
    public void LateUpdate()
    {
        if (_isMarking)
            UpdateAnchoredPos(_currentInteractable.InteractionMarkerPoint);
    }

    /// <summary>
    /// 상호작용 텍스트 가이드 UI를 활성화한다
    /// </summary>
    /// <param name="interactableObject"></param>
    public void EnableMarkerAt(InteractableObject interactableObject)
    {
        gameObject.SetActive(true);                     // UI 게임 오브젝트 활성화
        _isMarking = true;

        _currentInteractable = interactableObject;      // 현재 상호작용 중인 오브젝트 업데이트

        UpdateAnchoredPos(interactableObject.InteractionMarkerPoint);
    }
    /// <summary>
    /// 상호작용 텍스트 가이드 UI를 비활성화한다
    /// </summary>
    public void DisableMarker()
    {
        _isMarking = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 상호작용 텍스트 가이드의 위치를 업데이트한다
    /// </summary>
    /// <param name="targetWorldPos"></param>
    public void UpdateAnchoredPos(Vector3 targetWorldPos)
    {
        var markerPosAtScreen = Camera.main.WorldToScreenPoint(targetWorldPos);
        var centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f);

        _rectTransform.anchoredPosition = markerPosAtScreen - centerOfScreen;
    }
}