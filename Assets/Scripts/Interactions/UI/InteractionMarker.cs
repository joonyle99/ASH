using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMarker : MonoBehaviour
{
    RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>(); 
    }
    public void EnableAt(InteractableObject interactableObject)
    {
        gameObject.SetActive(true);
        _rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(interactableObject.InteractionMarkerPoint) - new Vector3(Screen.width/2, Screen.height/2);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
