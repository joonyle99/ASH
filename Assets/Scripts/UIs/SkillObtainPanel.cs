using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillObtainPanel : MonoBehaviour
{
    public struct SkillInfo
    {
        public Sprite Icon;
        public string MainText;
        public string DetailText;
    }
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _mainText;
    [SerializeField] TextMeshProUGUI _detailText;

    CanvasGroup _canvasGroup;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Open(SkillInfo info)
    {
        _icon.sprite = info.Icon;
        _mainText.text = info.MainText;
        _detailText.text = info.DetailText;

        gameObject.SetActive(true);
    }
    public void Close()
    {
        StartCoroutine(CloseCoroutine());
    }

    IEnumerator CloseCoroutine()
    {
        float duration = 1.5f;
        float eTime = 0f;
        while(eTime < duration)
        {
            _canvasGroup.alpha = 1 - eTime / duration;
            yield return null;
            eTime += Time.deltaTime;
        }
        gameObject.SetActive(false);
        _canvasGroup.alpha = 1;
    }
}
