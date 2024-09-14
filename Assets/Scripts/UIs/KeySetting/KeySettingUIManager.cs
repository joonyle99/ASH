using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class ChangeKeyCodeArgs : EventArgs
{
    public TMP_Text ButtonText;
    public InputSetterDataType InputSetterDataType;
    public CustomKeyCode TargetKeyCode;

    public ChangeKeyCodeArgs(TMP_Text buttonText, InputSetterDataType inputSetterDataType, CustomKeyCode targetKeyCode)
    {
        ButtonText = buttonText;
        InputSetterDataType = inputSetterDataType;
        TargetKeyCode = targetKeyCode;
    }
}

public enum ChangeKeySettingErrorReason
{
    None,
    AlreadyUsing,
    UnusableKey,
}

public class KeySettingUIManager : MonoBehaviour
{
    [SerializeField] RectTransform _scrollViewContent;

    [SerializeField] GameObject _keySettingBundlePrefab;

    [SerializeField] GameObject _keySettingBoxPrefab;

    [Header("UI Setting"), Tooltip("box생성 시 box의 크기는 margin과 content의 width에 의해 결정")]
    [SerializeField] float _boxLeftMargin = 10f;
    [SerializeField] float _boxRightMargin = 10f;
    [SerializeField] float _boxTopMargin = 10f;
    [SerializeField] float _boxBottomMargin = 10f;

    //파라미터로 박스 크기의 변화량을 브로드캐스트
    public UnityEvent<float, float> OnChangedContentBoxSize;

    private void Awake()
    {
        OnChangedContentBoxSize = new UnityEvent<float, float>();
        OnChangedContentBoxSize.AddListener(SetContentSize);
    }
    private void Start()
    {
        Init();
    }
    #region Initialize
    private void Init()
    {
        AddContentInScrollView(CreateKeySettingBundle(InputManager.Instance.DefaultInputSetter));
        AddContentInScrollView(CreateKeySettingBundle(InputManager.Instance.StayStillInputSetter));
    }

    private void AddContentInScrollView(GameObject content)
    {
        RectTransform contentRect = content.GetComponent<RectTransform>();
        content.transform.SetParent(_scrollViewContent.transform, false);

        _scrollViewContent.sizeDelta =
            new Vector2(_scrollViewContent.sizeDelta.x, _scrollViewContent.sizeDelta.y + contentRect.sizeDelta.y);
    }

    private GameObject CreateKeySettingBundle(InputSetterScriptableObject inputSetter)
    {
        List<DataDictionary<string, CustomKeyCode>> keyCodes = new List<DataDictionary<string, CustomKeyCode>>();

        switch(inputSetter.InputSetterDataType)
        {
            case InputSetterDataType.DefaultInputSetter:
                {
                    keyCodes = ((PCInputSetter)inputSetter).KeyCodes;
                    break;
                }
            case InputSetterDataType.StayStillInputSetter:
                {
                    keyCodes = ((StayStillInputSetter)inputSetter).KeyCodes;
                    break;
                }
        }

        RectTransform keySettingBundle = Instantiate(_keySettingBundlePrefab).GetComponent<RectTransform>();
        RectTransform label = keySettingBundle.transform.GetChild(0).Find("Label").GetComponent<RectTransform>();
        RectTransform content = keySettingBundle.transform.GetChild(0).Find("Content").GetComponent<RectTransform>();

        //label
        label.transform.Find("Description").GetComponent<TMP_Text>().text = inputSetter.name;
        label.GetComponentInChildren<Button>().onClick.AddListener(() => { ToggleDropDownBox(keySettingBundle); });

        //content
        float boxWidth = content.rect.width - (_boxLeftMargin + _boxRightMargin);
        float nextBoxPosX = _boxLeftMargin;
        float nextBoxPosY = -_boxTopMargin - label.sizeDelta.y;

        for(int i = 0; i < keyCodes.Count; i++)
        {
            GameObject newBox = CreateKeySettingBox(inputSetter.InputSetterDataType, keyCodes[i].Value);
            RectTransform boxRect = newBox.GetComponent<RectTransform>();

            boxRect.localPosition = new Vector3(nextBoxPosX, nextBoxPosY);
            boxRect.sizeDelta = new Vector2(boxWidth, boxRect.sizeDelta.y);
            newBox.transform.parent = content.transform;

            nextBoxPosY -= boxRect.sizeDelta.y + _boxBottomMargin + _boxTopMargin;
        }

        //set content size
        content.sizeDelta = new Vector2(content.sizeDelta.x, -nextBoxPosY - _boxTopMargin);
        keySettingBundle.sizeDelta =
            new Vector2(keySettingBundle.sizeDelta.x, content.sizeDelta.y + label.sizeDelta.y);

        return keySettingBundle.gameObject;
    }

    private GameObject CreateKeySettingBox(InputSetterDataType inputSetterDataType, CustomKeyCode keyCode)
    {
        GameObject newBox = Instantiate(_keySettingBoxPrefab);
        TMP_Text actionText = newBox.transform.GetChild(0).Find("Text_Action").GetComponent<TMP_Text>();
        Button button = newBox.transform.GetChild(0).Find("Button").GetComponent<Button>();
        TMP_Text buttonText = newBox.transform.GetChild(0).Find("Button").GetComponentInChildren<TMP_Text>();

        button.onClick.AddListener(() => OnChangedKeyboardSetting(this, new ChangeKeyCodeArgs(buttonText, inputSetterDataType, keyCode)));
        actionText.text = keyCode.Name;
        buttonText.text = keyCode.KeyCode.ToString();

        return newBox;
    }
    #endregion

    #region Function

    private void SetContentSize(float x, float y)
    {
        _scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x + x, _scrollViewContent.sizeDelta.y + y);
    }

    public void ToggleKeySettingPanel()
    {
        bool value = !transform.Find("Window").gameObject.activeSelf;

        transform.Find("Window").gameObject.SetActive(value);
    }

    public void ToggleDropDownBox(RectTransform keySettingBundle)
    {
        RectTransform labelRect = keySettingBundle.transform.GetChild(0).Find("Label").GetComponent<RectTransform>();
        RectTransform contentRect = keySettingBundle.transform.GetChild(0).Find("Content").GetComponent<RectTransform>();

        if (keySettingBundle.sizeDelta.y == labelRect.sizeDelta.y)
        {
            keySettingBundle.sizeDelta = new Vector2(keySettingBundle.sizeDelta.x,
                labelRect.sizeDelta.y + contentRect.sizeDelta.y);
            OnChangedContentBoxSize?.Invoke(0, contentRect.sizeDelta.y);
        }
        else
        {
            keySettingBundle.sizeDelta = new Vector2(keySettingBundle.sizeDelta.x,
                labelRect.sizeDelta.y);
            OnChangedContentBoxSize?.Invoke(0, -contentRect.sizeDelta.y);
        }
    }

    private string KeySettingErrorString(ChangeKeySettingErrorReason value)
    {
        switch (value)
        {
            case ChangeKeySettingErrorReason.None:
                return "None";
            case ChangeKeySettingErrorReason.AlreadyUsing:
                return "이미 사용중인 키 입니다";
            case ChangeKeySettingErrorReason.UnusableKey:
                return "사용 불가능한 키 입니다";
        }

        return "";
    }
    #endregion

    #region Events
    private void OnChangedKeyboardSetting(object sender, ChangeKeyCodeArgs changeKeyCodeArgs)
    {
        StartCoroutine(ChangeKeyboardSettingLogic(this, changeKeyCodeArgs));
    }

    private IEnumerator ChangeKeyboardSettingLogic(object sender, ChangeKeyCodeArgs changeKeyCodeArgs)
    {
        changeKeyCodeArgs.ButtonText.text = "사용하실 키를 입력해 주세요.";
        yield return null;

        while (true)
        {
            if(Input.anyKeyDown)
            {
                foreach (KeyCode kCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kCode))
                    {
                        ChangeKeySettingErrorReason changeKeySettingErrorReason = KeySettingManager.CheckKeyCode(changeKeyCodeArgs.TargetKeyCode.Name, kCode);
                        string errorString = KeySettingErrorString(changeKeySettingErrorReason);

                        if (changeKeySettingErrorReason == ChangeKeySettingErrorReason.None)
                        {
                            KeySettingManager.SetKeyboardSetting(changeKeyCodeArgs.InputSetterDataType,
                            changeKeyCodeArgs.TargetKeyCode.Name, kCode);
                            changeKeyCodeArgs.ButtonText.text = kCode.ToString();
                        }
                        else
                        {
                            //changeKeyCodeArgs.ButtonText.text = errorString;
                        }
                    }
                }
                changeKeyCodeArgs.ButtonText.text = changeKeyCodeArgs.TargetKeyCode.KeyCode.ToString();

                yield break;
            }

            yield return null;
        }
    }
    #endregion
}
