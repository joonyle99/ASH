using UnityEngine;

/// <summary>
/// ��ȣ�ۿ� �ؽ�Ʈ ���̵带 ����ϱ� ���� UI
///
/// ��ȣ�ۿ� ��Ŀ UI ����� ��
///     1. �÷��̾ ��ȣ�ۿ� ������ ������Ʈ�� Ʈ���� �ڽ��� �� ���
///     2. �÷��̾�� ������Ʈ�� ��ȣ�ۿ� ���� ���
///     3. �÷��̾�� ������Ʈ�� ��ȣ�ۿ��� �Ÿ��� �־��� ���� ���
///     4. �÷��̾ ��ȣ�ۿ��� ����ģ ���
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
            // 2. �÷��̾�� ������Ʈ�� ��ȣ�ۿ� ���� ��� (feat. InteractionMarker.cs)
            if (_currentObject.IsInteracting)
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
        // ���:
        //     ��ȣ�ۿ� �ؽ�Ʈ ���̵��� ��ġ ������ Update()���� ���� '��ȣ�ۿ� ������ ������Ʈ�� ������ ��'�� ����Ѵ�
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
}