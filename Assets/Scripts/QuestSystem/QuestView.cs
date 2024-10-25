using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CanvasGroup = UnityEngine.CanvasGroup;

/// <summary>
/// 퀘스트 데이터를 전달받아 뷰 UI에 표시하는 클래스
/// </summary>
public class QuestView : MonoBehaviour, ISceneContextBuildListener
{
    public Image QuestPanel;
    public TextMeshProUGUI Counter;

    [Space]

    public float FadeTime = 1.5f;

    public void OnSceneContextBuilt()
    {
        
        // * TODO: 퀘스트 시스템 완성하기
        var currentQuest = QuestController.Instance.CurrentQuest;

        if (currentQuest.QuestData != null && currentQuest.IsActive)
        {
            UpdatePanel(currentQuest);
            OpenPanel();
        }
        
    }

    public void OpenPanel()
    {
        StartCoroutine(ControlPanelCoroutine(true));
    }
    public void ClosePanel()
    {
        StartCoroutine(ControlPanelCoroutine(false));
    }
    public void UpdatePanel(Quest quest)
    {
        Debug.Log("Quest argument in QuestView 'UpdatePanel()' Function : " + quest.QuestData);

        Counter.text = quest.CurrentCount.ToString()
                + " / " + quest.QuestData.GoalCount.ToString();
    }
    private IEnumerator ControlPanelCoroutine(bool isOpen, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);

        var canvasGroup = QuestPanel.GetComponent<CanvasGroup>();

        var eTime = 0f;

        if (isOpen)
        {
            canvasGroup.alpha = 0f;

            canvasGroup.gameObject.SetActive(true);

            while (eTime < FadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, eTime / FadeTime);
                yield return null;
                eTime += Time.deltaTime;
            }

            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 1f;

            while (eTime < FadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, eTime / FadeTime);
                yield return null;
                eTime += Time.deltaTime;
            }

            canvasGroup.alpha = 0f;

            canvasGroup.gameObject.SetActive(false);
        }
    }
}
