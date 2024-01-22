using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemObtainPanel : MonoBehaviour
{
    public struct ItemObtainInfo
    {
        public Sprite Icon;
        public string MainText;
        public string DetailText;
    }
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _mainText;
    [SerializeField] TextMeshProUGUI _detailText;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void Open(ItemObtainInfo info)
    {
        if (info.Icon != null) 
            _icon.sprite = info.Icon;
        _mainText.text = info.MainText;
        _detailText.text = info.DetailText;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        _animator.SetTrigger("Close");
    }
    public void AnimEvent_CloseDone()
    {
        gameObject.SetActive(false);
    }
}
