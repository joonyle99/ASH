using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class KeySettingBox : MonoBehaviour
{
    //키세팅 변경 시 키세팅 버튼을 클릭하게 되면,
    //OnChangedKeyboardSetting에서 키 입력을 받고 그 즉시 다시 OnClick버튼이 클릭되어 로직이 재실행 됨.
    //이를 확인하기 위한 변수
    public int MouseClickCount = 0;

    public string ActionKey = "";

    private float _widthOffset = 50f;

    [SerializeField] private TMP_Text _action;

    [SerializeField] private TMP_Text _keyText;
    [SerializeField] private Image _keyImg;

    [SerializeField] private Button _changeKeyButton;
    public Button ChangeKeyButton => _changeKeyButton;

    private CustomKeyCode _keyCode;

    public void InitKey(string key, CustomKeyCode keyCode)
    {
        _keyCode = keyCode;
        ActionKey = key;
        if(keyCode == null)
        {
            _keyText.text = "";

            _keyText.gameObject.SetActive(true);
            _keyImg.gameObject.SetActive(false);
            return;
        }

        _action.text = UITranslator.GetLocalizedString(keyCode.NameKey);
        SetKeyText(keyCode.KeyCode);
    }

    public void SetKeyText(KeyCode newKey)
    {
        Sprite buttonImage = KeySettingUIManager.Instance.FindImageButtonBox(newKey);

        if (buttonImage != null)
        {
            _keyImg.sprite = buttonImage;
            SetButtonWidth(20f + _widthOffset);
            _keyText.gameObject.SetActive(false);
            _keyImg.gameObject.SetActive(true);
        }
        else
        {
            if (newKey == KeyCode.None)
            {
                _keyText.text = "";
                SetButtonWidth(50f + _widthOffset);
            }
            else
            {
                _keyText.text = newKey.ToString();
                SetButtonWidth(20f * _keyText.text.Length + _widthOffset);
            }

            _keyText.gameObject.SetActive(true);
            _keyImg.gameObject.SetActive(false);
        }
    }

    private void SetButtonWidth(float width)
    {
        RectTransform buttonRect = _changeKeyButton.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(width, buttonRect.sizeDelta.y);
    }

    public void BindOnChangeKeyButton(UnityAction action)
    {
        _changeKeyButton.onClick.AddListener(action);
    }

    public void ApplyActionText()
    {
        _action.text = UITranslator.GetLocalizedString(_keyCode.NameKey);
    }
}
