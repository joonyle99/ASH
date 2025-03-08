using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EndCredit : MonoBehaviour
{
    [SerializeField] private float _maxRectY = 1000f;

    public TMP_Text CreditsText;
    public Image Panel;

    private void Start()
    {
        Panel.gameObject.SetActive(false);
    }

    public void StartEndCredits()
    {
        Panel.gameObject.SetActive(true);
        StartCoroutine(StartEndCreditsTextLogic(60f));
        StartCoroutine(StartEndCreditsFadeOutLogic(10f));
    }

    private IEnumerator StartEndCreditsTextLogic(float duration)
    {
        float timer = 0;
        float rectYOffset = CreditsText.rectTransform.position.y;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            yield return null;

            float newPositionY = Mathf.Lerp(rectYOffset, _maxRectY, timer / duration);
            Vector3 newTextPosition = new Vector3(CreditsText.rectTransform.position.x, newPositionY, CreditsText.rectTransform.position.z);
            CreditsText.rectTransform.position = newTextPosition;
        }
    }

    private IEnumerator StartEndCreditsFadeOutLogic(float duration)
    {
        float timer = 0;
        float panelAlphaOffset = Panel.color.a;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            yield return null;

            float newPanelAlpha = Mathf.Lerp(panelAlphaOffset, 1f, timer / duration);
            Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, newPanelAlpha);
        }
    }
}
