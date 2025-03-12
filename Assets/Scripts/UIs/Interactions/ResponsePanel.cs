using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ResponseButtonType
{
    None,
    Accept,
    Reject,
}

public class ResponseContainer
{
    public ResponseButtonType buttonType = ResponseButtonType.None;
    public UnityAction action;

    public ResponseContainer(ResponseButtonType buttonType, UnityAction action)
    {
        this.buttonType = buttonType;
        this.action = action;
    }
}

public class ResponsePanel : MonoBehaviour
{
    private Button _accept;
    private Button _reject;

    private TextMeshProUGUI _acceptText;
    private TextMeshProUGUI _rejectText;

    public Button Accept => _accept;
    public Button Reject => _reject;

    public TextMeshProUGUI AcceptText
    {
        get => _acceptText;
        set => _acceptText = value;
    }
    public TextMeshProUGUI RejectText
    {
        get => _rejectText;
        set => _rejectText = value;
    }

    private void Awake()
    {
        _accept = transform.Find("Accept").GetComponent<Button>();
        _acceptText = _accept.GetComponentInChildren<TextMeshProUGUI>();
        _reject = transform.Find("Reject").GetComponent<Button>();
        _rejectText = _reject.GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// 여러 개체가 공용으로 사용하는 UI이므로 panel이 닫히면 모든 버튼의 리스너를 삭제
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
