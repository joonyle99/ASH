using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemObtainPanel : MonoBehaviour
{
    [Serializable]
    public struct ItemObtainInfo
    {
        public Sprite Icon;
        public string MainText;
        public string DetailText;
    }

    [SerializeField] Image _icon;
    [SerializeField] Image _mainTextImage;
    [SerializeField] TextMeshProUGUI _mainText;
    [SerializeField] TextMeshProUGUI _detailText;

    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void Open(ItemObtainInfo info)
    {
        //스킬 조각인 경우 메인 텍스트가 이미지로 들어가 있음
        if (info.MainText == "")
        {
            _mainTextImage.gameObject.SetActive(true);
        }
        else
        {
            _mainText.text = info.MainText;
            _mainText.gameObject.SetActive(true);
            _mainTextImage.gameObject.SetActive(false);
        }

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
