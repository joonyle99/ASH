using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ResponseButtonType
{
    None,
    Accept,
    Reject,
}

public class ResponseFunctionContainer
{
    public ResponseButtonType buttonType = ResponseButtonType.None;
    public UnityAction action;

    public ResponseFunctionContainer(ResponseButtonType buttonType, UnityAction action)
    {
        this.buttonType = buttonType;
        this.action = action;
    }
}

public class ResponsePanel : MonoBehaviour
{

    private Button _accept;
    private Button _reject;

    private void Awake()
    {
        _accept = transform.Find("Accept").GetComponent<Button>();
        _reject = transform.Find("Reject").GetComponent<Button>();
    }

    /// <summary>
    /// ���� ��ü�� �������� ����ϴ� UI�̹Ƿ� panel�� ������ ��� ��ư�� �����ʸ� ����
    /// </summary>
    private void OnDisable()
    {
        UnbindActionOnClicked(ResponseButtonType.Accept, () => { }, true);
        UnbindActionOnClicked(ResponseButtonType.Reject, () => { }, true);
    }

    public void BindActionOnClicked(ResponseButtonType responseButtonType, UnityAction action)
    {
        switch (responseButtonType)
        {
            case ResponseButtonType.Accept:
                {
                    _accept.onClick.AddListener(action);
                    break;
                }
            case ResponseButtonType.Reject:
                {
                    _reject.onClick.AddListener(action);
                    break;
                }
        }
    }

    public void UnbindActionOnClicked(ResponseButtonType responseButtonType, UnityAction action, bool isRemoveAll)
    {
        switch (responseButtonType)
        {
            case ResponseButtonType.Accept:
                {
                    if(isRemoveAll)
                        _accept.onClick.RemoveAllListeners();
                    else
                        _accept.onClick.RemoveListener(action);
                    break;
                }
            case ResponseButtonType.Reject:
                {
                    if(isRemoveAll)
                        _reject.onClick.RemoveAllListeners();
                    else
                        _reject.onClick.RemoveListener(action);
                    break;
                }
        }
    }
}
