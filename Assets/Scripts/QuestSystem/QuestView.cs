using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CanvasGroup = UnityEngine.CanvasGroup;

/// <summary>
/// 퀘스트 데이터를 전달받아 뷰 UI에 표시하는 클래스
/// </summary>
public class QuestView : MonoBehaviour
{
    public Image QuestPanel;
    public TextMeshProUGUI Counter;

    public void OpenQuestPanel()
    {
        StartCoroutine(ControlPanelCoroutine(true));
    }

    public void ClosePanel()
    {
        StartCoroutine(ControlPanelCoroutine(false));
    }

    private IEnumerator ControlPanelCoroutine(bool isOpen, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);

        var canvasGroup = QuestPanel.GetComponent<CanvasGroup>();

        var eTime = 0f;
        var duration = 1.5f;

        if (isOpen)
        {
            canvasGroup.alpha = 0f;

            canvasGroup.gameObject.SetActive(true);

            while (eTime < duration)
            {
                yield return null;
                eTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, eTime / duration);
            }

            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 1f;

            while (eTime < duration)
            {
                yield return null;
                eTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, eTime / duration);
            }

            canvasGroup.alpha = 0f;

            canvasGroup.gameObject.SetActive(false);
        }
    }

    public void DrawUpdateDataOnPanel(QuestData questData)
    {
        Counter.text = questData.MonsterQuest.MonsterKilled.ToString() + " / " + questData.MonsterQuest.KillGoal.ToString();
    }
}
