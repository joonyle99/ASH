using UnityEngine;

/// <summary>
/// ��ȣ�ۿ� �ؽ�Ʈ ���̵带 ����ϱ� ���� UI
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
        // ���:
        //     ��ȣ�ۿ� �ؽ�Ʈ ���̵��� ��ġ ������ Update()���� ���� '��ȣ�ۿ� ������ ������Ʈ�� ������ ��'�� ����Ѵ�
        //

        if (_isMarking)
            UpdateMarkerPos(_currentObject.InteractionMarkerPoint);
    }

    public void EnableMarkerAt(InteractableObject interactionObject)
    {
        gameObject.SetActive(true);                     // UI ���� ������Ʈ Ȱ��ȭ
        _isMarking = true;

        _currentObject = interactionObject;             // ���� ��ȣ�ۿ� ���� ������Ʈ ������Ʈ

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