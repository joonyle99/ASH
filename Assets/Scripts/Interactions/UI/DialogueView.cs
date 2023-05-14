using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueView : MonoBehaviour
{
    [SerializeField] Image _dialoguePanel;
    [SerializeField] TextMeshProUGUI _dialogueText;

    public void OpenPanel()
    {
        _dialoguePanel.gameObject.SetActive(true);
    }
    public void SetText(string text)
    {
        _dialogueText.text = text;
    }
    public void ClosePanel()
    {
        _dialoguePanel.gameObject.SetActive(false);
    }
}
