using UnityEngine;
using UnityEngine.EventSystems;

public class CustomEventTrigger : EventTrigger
{
    private Animator _animator;

    public static bool isPointerInside = false;
    public static bool isPointerPressing = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Called by the EventSystem when the pointer enters the object associated with this EventTrigger.
    /// </summary>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        _animator.SetBool("IsPointerInside", isPointerInside);

        if (isPointerPressing) return;

        base.OnPointerEnter(eventData);
    }

    /// <summary>
    /// Called by the EventSystem when the pointer exits the object associated with this EventTrigger.
    /// </summary>
    public override void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        _animator.SetBool("IsPointerInside", isPointerInside);

        if (isPointerPressing) return;

        base.OnPointerExit(eventData);
    }

    /// <summary>
    /// Called by the EventSystem when a PointerDown event occurs.
    /// </summary>
    public override void OnPointerDown(PointerEventData eventData)
    {
        isPointerPressing = true;
        _animator.SetBool("IsPointerPressing", isPointerPressing);

        base.OnPointerDown(eventData);
    }

    /// <summary>
    /// Called by the EventSystem when a PointerUp event occurs.
    /// </summary>
    public override void OnPointerUp(PointerEventData eventData)
    {
        isPointerPressing = false;
        _animator.SetBool("IsPointerPressing", isPointerPressing);

        base.OnPointerUp(eventData);
    }

    /// <summary>
    /// Called by the EventSystem when a Click event occurs.
    /// </summary>
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }
}
