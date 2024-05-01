using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public Rect rect;
    public string dialogueText;
    public List<string> choices;

    public void Draw()
    {
        GUI.Box(rect, dialogueText);
        foreach (var choice in choices)
        {
            GUILayout.BeginArea(rect);
            if (GUILayout.Button(choice))
            {
                // ������ Ŭ�� �� ����
            }
            GUILayout.EndArea();
        }
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(e.mousePosition))
                {
                    // ��� ���� ����
                    return true;
                }
                break;
        }
        return false;
    }
}
