using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialZone : TriggerZone
{
    private Dictionary<KeyCode, string> _displayReplaceKeyCode = new Dictionary<KeyCode, string>();
    [SerializeField] private List<string> _keyCodeName = new();

    [SerializeField] private GameObject skillObject;
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOffDuration = 0.3f;

    private float _originalAlpha = 1f;

    private Image[] _images;
    private TextMeshProUGUI[] _texts;

    private void Awake()
    {
        _displayReplaceKeyCode.Add(KeyCode.RightArrow, "กๆ");
        _displayReplaceKeyCode.Add(KeyCode.LeftArrow, "ก็");
        _displayReplaceKeyCode.Add(KeyCode.UpArrow, "ก่");
        _displayReplaceKeyCode.Add(KeyCode.DownArrow, "ก้");
        _displayReplaceKeyCode.Add(KeyCode.Mouse0, "Mouse Left");
        _displayReplaceKeyCode.Add(KeyCode.Mouse1, "Mouse Right");

        // get every image and text
        _images = skillObject.GetComponentsInChildren<Image>();
        _texts = skillObject.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var image in _images)
        {
            Color tempColor = image.color;
            tempColor.a = 0f;
            image.color = tempColor;
        }
        foreach (var textMesh in _texts)
        {
            Color tempColor = textMesh.color;
            tempColor.a = 0f;
            textMesh.color = tempColor;
        }
    }

    public override void OnPlayerEnter(PlayerBehaviour player)
    {
        StartCoroutine(FadeAllCoroutine(0f, _originalAlpha, _fadeInDuration));
    }

    public override void OnPlayerStay(PlayerBehaviour player)
    {
        base.OnPlayerStay(player);

        UpdateKeyCodeText();
    }

    public override void OnPlayerExit(PlayerBehaviour player)
    {
        StartCoroutine(FadeAllCoroutine(_originalAlpha, 0f, _fadeOffDuration));
    }
    IEnumerator FadeAllCoroutine(float from, float to, float duration)
    {
        float eTime = 0f;
        while (eTime <= duration)
        {
            Color[] tempColors = new Color[_images.Length + _texts.Length];

            for (int i = 0; i < _images.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[i] = _images[i].color;
                tempColors[i].a = alpha;
                _images[i].color = tempColors[i];
            }
            for (int i = 0; i < _texts.Length; i++)
            {
                float alpha = Mathf.Lerp(from, to, eTime / duration);
                tempColors[_images.Length + i] = _texts[i].color;
                tempColors[_images.Length + i].a = alpha;
                _texts[i].color = tempColors[_images.Length + i];
            }

            yield return null;

            eTime += Time.deltaTime;
        }

        // set final alpha
        foreach (var image in _images)
        {
            var color = image.color;
            color.a = to;
            image.color = color;
        }
        foreach (var text in _texts)
        {
            var color = text.color;
            color.a = to;
            text.color = color;
        }
    }

    public void UpdateKeyCodeText()
    {
        if(skillObject != null)
        {
            List<TMP_Text> keyText = new List<TMP_Text>();
            for(int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                Transform keyBox = transform.GetChild(0).GetChild(i);
                if (keyBox.tag == "KeyBox")
                {
                    keyText.Add(keyBox.GetComponentInChildren<TMP_Text>());
                }
            }

            PCInputSetter pcInputSetter = InputManager.Instance.DefaultInputSetter as PCInputSetter;
            
            for(int i = 0; i < keyText.Count; i++)
            {
                KeyCode newKeyCode = pcInputSetter.GetKeyCode(_keyCodeName[i]).KeyCode;

                if (newKeyCode == KeyCode.None) continue;


                if (_displayReplaceKeyCode.ContainsKey(newKeyCode))
                {
                    keyText[i].text = _displayReplaceKeyCode[newKeyCode];
                }else
                {
                    keyText[i].text = newKeyCode.ToString();
                }
            }
        }
    }
}
