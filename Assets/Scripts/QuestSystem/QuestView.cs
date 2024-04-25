using TMPro;
using UnityEngine;

public class QuestView : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Number;

    public void Open(Quest quest)
    {
        gameObject.SetActive(true);

        Description.text = quest.Description;
        Title.text = quest.Title;
        Number.text = quest.Title;
    }
}
