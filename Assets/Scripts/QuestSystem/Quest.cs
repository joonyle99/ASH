/// <summary>
/// Äù½ºÆ® Á¤º¸
/// </summary>
[System.Serializable]
public class Quest
{
    public enum QuestType
    {
        Collect,
        Kill,
        Talk
    }

    public bool isActive;

    public string title;
    public QuestType type;
    public string description;
    public int reward;

    // public QuestGOal goal;

}