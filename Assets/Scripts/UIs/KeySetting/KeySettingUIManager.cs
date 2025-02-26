using HappyTools;
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
    public KeySettingBox KeySettingBox;
    public InputSetterDataType InputSetterDataType;
    public CustomKeyCode TargetKeyCode;

    public ChangeKeyCodeArgs(KeySettingBox keySettingBox, InputSetterDataType inputSetterDataType, CustomKeyCode targetKeyCode)
    {
        KeySettingBox = keySettingBox;
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

public class KeySettingUIManager : SingletonBehaviourFixed<KeySettingUIManager>
{
    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject _OverlappedKeyWarningText;

    private bool _waitAnyKeyDown = false;

    public GameObject ColumnKeyBox1;
    public GameObject ColumnKeyBox2;

    private List<KeySettingBox> _keyBoxes = new List<KeySettingBox>();

    [SerializeField] private List<DataDictionary<KeyCode, Sprite>> _fillIntoButtonWithImages = new List<DataDictionary<KeyCode, Sprite>>();

    private void Start()
    {
        foreach (var keyBox in ColumnKeyBox1.GetComponentsInChildren<KeySettingBox>())
        {
            _keyBoxes.Add(keyBox);
        }
        foreach (var keyBox in ColumnKeyBox2.GetComponentsInChildren<KeySettingBox>())
        {
            _keyBoxes.Add(keyBox);
        }

        int keyBoxIdx = 0;
        foreach (var keyCodeData in InputManager.Instance.DefaultInputSetter.KeyCodes)
        {
            KeySettingBox keySettingBox = _keyBoxes[keyBoxIdx];
            keySettingBox.InitKey(keyCodeData.Key, keyCodeData.Value);
            keySettingBox.BindOnChangeKeyButton(
                () => OnChangedKeyboardSetting(this, new ChangeKeyCodeArgs(keySettingBox, InputSetterDataType.DefaultInputSetter, keyCodeData.Value)));

            keyBoxIdx++;
        }
    }

    private void Update()
    {
        if (InputManager.Instance.State.EscapeKey.KeyDown && !_waitAnyKeyDown)
        {
            var PlayablePlayer = SceneContext.Current.PlayableSceneTransitionPlayer;
            if (PlayablePlayer != null && PlayablePlayer.IsPlayable == false)
            {
                return;
            }

            ClosePanel();
        }
    }
    #region Initialize

    #endregion

    #region Function
    public void ToggleKeySettingPanel()
    {
        bool value = !_window.gameObject.activeSelf;

        if(value)
        {
            _window.gameObject.SetActive(value);
        }
        else
        {
            ClosePanel();
        }
    }

    public void ClosePanel()
    {
        _window.gameObject.SetActive(false);
    }

    public bool IsOpened()
    {
        return _window.gameObject.activeSelf;
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
        Debug.Log($"{changeKeyCodeArgs.TargetKeyCode.Name} action key pressed");
        if (changeKeyCodeArgs.KeySettingBox.MouseClickCount >= 1)
        {
            changeKeyCodeArgs.KeySettingBox.MouseClickCount = 0;
            return;
        }

        changeKeyCodeArgs.KeySettingBox.ChangeKeyButton.interactable = false;
        StartCoroutine(ChangeKeyboardSettingLogic(this, changeKeyCodeArgs));
    }

    private IEnumerator ChangeKeyboardSettingLogic(object sender, ChangeKeyCodeArgs changeKeyCodeArgs)
    {
        changeKeyCodeArgs.KeySettingBox.SetKeyText(KeyCode.None);
        yield return null;

        _waitAnyKeyDown = true;
        while (true)
        {
            if(Input.anyKeyDown)
            {
                foreach (KeyCode kCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kCode))
                    {
                        if(kCode == KeyCode.Escape)
                        {
                            changeKeyCodeArgs.KeySettingBox.SetKeyText(changeKeyCodeArgs.TargetKeyCode.KeyCode);
                        }
                        else
                        {
                            if (kCode == KeyCode.Mouse0)
                            {
                                changeKeyCodeArgs.KeySettingBox.MouseClickCount++;
                                Debug.Log("ChangeKeyboard Setting Logic in Mouse click Count : " + changeKeyCodeArgs.KeySettingBox.MouseClickCount);
                            }

                            ChangeKeySettingErrorReason changeKeySettingErrorReason = KeySettingManager.CheckKeyCode(changeKeyCodeArgs.TargetKeyCode.Name, kCode);
                            string errorString = KeySettingErrorString(changeKeySettingErrorReason);
                            if (changeKeySettingErrorReason == ChangeKeySettingErrorReason.None)
                            {
                                KeySettingManager.SetKeyboardSetting(changeKeyCodeArgs.InputSetterDataType,
                                changeKeyCodeArgs.TargetKeyCode.Key, kCode);
                                changeKeyCodeArgs.KeySettingBox.SetKeyText(kCode);
                            }
                            else if(changeKeySettingErrorReason == ChangeKeySettingErrorReason.AlreadyUsing)
                            {
                                // 새로 누른 키가 자신이 아닌 경우만 로직 진행
                                if(kCode != changeKeyCodeArgs.TargetKeyCode.KeyCode)
                                {
                                    _OverlappedKeyWarningText.gameObject.SetActive(true);

                                    //사용중인 키 변경 원할 때 재확인
                                    while (true)
                                    {
                                        yield return null;

                                        if (Input.anyKeyDown)
                                        {
                                            foreach (KeyCode checkKeyCode in Enum.GetValues(typeof(KeyCode)))
                                            {
                                                if (Input.GetKey(checkKeyCode))
                                                {
                                                    //키 변경
                                                    if (checkKeyCode == kCode)
                                                    {
                                                        // 기존 사용중이던 키 삭제
                                                        string actionKey = KeySettingManager.GetActionKeyByKeyCode(changeKeyCodeArgs.InputSetterDataType, checkKeyCode);
                                                        KeySettingManager.SetKeyboardSetting(changeKeyCodeArgs.InputSetterDataType, actionKey, KeyCode.None);
                                                        GetKeySettingBox(actionKey)?.SetKeyText(KeyCode.None);

                                                        // 키 변경
                                                        KeySettingManager.SetKeyboardSetting(changeKeyCodeArgs.InputSetterDataType,
                                                        changeKeyCodeArgs.TargetKeyCode.Key, kCode);
                                                        changeKeyCodeArgs.KeySettingBox.SetKeyText(kCode);

                                                    }
                                                }
                                            }

                                            break;
                                        }
                                    }

                                    _OverlappedKeyWarningText.gameObject.SetActive(false);
                                }
                                else
                                {
                                    changeKeyCodeArgs.KeySettingBox.SetKeyText(changeKeyCodeArgs.TargetKeyCode.KeyCode);
                                }
                            }
                            else
                            {
                                Debug.Log("Fail to Change Key reason : " + errorString);
                            }
                        }
                    }
                }

                changeKeyCodeArgs.KeySettingBox.ChangeKeyButton.interactable = true;

                _waitAnyKeyDown = false;
                yield break;
            }

            yield return null;
        }
    }

    private KeySettingBox GetKeySettingBox(string key)
    {
        for(int i = 0; i < _keyBoxes.Count; i++)
        {
            if (_keyBoxes[i].ActionKey == key)
            {
                return _keyBoxes[i];
            }
        }

        return null;
    }

    public Sprite FindImageButtonBox(KeyCode keyCode)
    {
        for (int i = 0; i < _fillIntoButtonWithImages.Count; i++)
        {
            if (keyCode == _fillIntoButtonWithImages[i].Key)
            {
                return _fillIntoButtonWithImages[i].Value;
            }
        }

        return null;
    }
    #endregion
}
