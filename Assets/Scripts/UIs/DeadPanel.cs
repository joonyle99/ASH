using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadPanel : MonoBehaviour
{
    [SerializeField] private GameObject _window;

    [SerializeField] private Image _panel;
    [SerializeField] private TMP_Text _text;
    //수락 시 부활
    [SerializeField] private Button _buttonAccept;
    //거절 시 메인화면 이동
    //[SerializeField] private Button _buttonReject;

    private void Start()
    {
        //_buttonReject.onClick.AddListener(GameUIManager.Instance.OptionView.GoToTitleScene);
    }

    public void Open()
    {
        StartCoroutine(OpenFadeLogic());

        _buttonAccept.onClick.RemoveAllListeners();
        _buttonAccept.onClick.AddListener(PlayerBehaviour.Respawn);
    }

    private IEnumerator OpenFadeLogic()
    {
        float textDisplayWaitTime = 1f;

        _window.SetActive(true);
        _buttonAccept.gameObject.SetActive(false);
        //_buttonReject.gameObject.SetActive(false);
        _text.gameObject.SetActive(false);

        yield return FadeWindow(2f);

        yield return new WaitForSeconds(textDisplayWaitTime);

        _buttonAccept.gameObject.SetActive(true);
        //_buttonReject.gameObject.SetActive(true);
        _text.gameObject.SetActive(true);
    }

    private IEnumerator FadeWindow(float duration)
    {
        float timer = 0f;
        Color panelColor = new Color(0, 0, 0, 0);

        while (timer < duration) 
        {
            panelColor.a = Mathf.Lerp(0, 1, timer / duration);
            _panel.color = panelColor;

            yield return null;
            timer += Time.deltaTime;
        }

        panelColor.a = 1;
        _panel.color = panelColor;
    }

    public void Close()
    {
        _window.SetActive(false);
    }

    private void OnDestroy()
    {
        _buttonAccept.onClick?.RemoveAllListeners();
        //_buttonReject.onClick?.RemoveAllListeners();
    }
}
