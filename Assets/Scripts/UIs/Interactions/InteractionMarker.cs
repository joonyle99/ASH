using UnityEngine;

/// <summary>
/// ��ȣ�ۿ� �ؽ�Ʈ ���̵带 ����ϱ� ���� UI
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
    /// ��ȣ�ۿ� �ؽ�Ʈ ���̵��� ��ġ ������ Update()���� ���� ��ȣ�ۿ� ������ ������Ʈ�� ������ �Ŀ� ����Ѵ�
    /// </summary>
    public void LateUpdate()
    {
        if (_isMarking)
            UpdateAnchoredPos(_currentInteractable.InteractionMarkerPoint);
    }

    /// <summary>
    /// ��ȣ�ۿ� �ؽ�Ʈ ���̵� UI�� Ȱ��ȭ�Ѵ�
    /// </summary>
    /// <param name="interactableObject"></param>
    public void EnableMarkerAt(InteractableObject interactableObject)
    {
        gameObject.SetActive(true);                     // UI ���� ������Ʈ Ȱ��ȭ
        _isMarking = true;

        _currentInteractable = interactableObject;      // ���� ��ȣ�ۿ� ���� ������Ʈ ������Ʈ

        UpdateAnchoredPos(interactableObject.InteractionMarkerPoint);
    }
    /// <summary>
    /// ��ȣ�ۿ� �ؽ�Ʈ ���̵� UI�� ��Ȱ��ȭ�Ѵ�
    /// </summary>
    public void DisableMarker()
    {
        _isMarking = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��ȣ�ۿ� �ؽ�Ʈ ���̵��� ��ġ�� ������Ʈ�Ѵ�
    /// </summary>
    /// <param name="targetWorldPos"></param>
    public void UpdateAnchoredPos(Vector3 targetWorldPos)
    {
        var markerPosAtScreen = Camera.main.WorldToScreenPoint(targetWorldPos);
        var centerOfScreen = new Vector3(Screen.width / 2f, Screen.height / 2f);

        _rectTransform.anchoredPosition = markerPosAtScreen - centerOfScreen;
    }
}