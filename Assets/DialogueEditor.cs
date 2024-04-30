using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueEditor : EditorWindow
{
    private List<DialogueNode> nodes = new List<DialogueNode>();
    private DialogueNode selectedNode;

    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowEditor()
    {
        DialogueEditor window = GetWindow<DialogueEditor>("Dialogue Editor");
        window.Show();
    }

    void OnEnable()
    {
        // 초기 노드 로드 또는 생성
        // Example: nodes.Add(new DialogueNode { rect = new Rect(10, 10, 200, 150), dialogueText = "Example Node" });
        nodes.Add(new DialogueNode { rect = new Rect(10, 10, 200, 150), dialogueText = "Example Node" });
    }

    void OnGUI()
    {
        DrawNodes();
        // ProcessNodeEvents(Event.current);
        // ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    void DrawNodes()
    {
        foreach (DialogueNode node in nodes)
        {
            node.Draw();
        }
    }

    void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
        }
    }

    void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ClearConnectionSelection()
    {
        selectedNode = null;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        nodes.Add(new DialogueNode { rect = new Rect(mousePosition.x, mousePosition.y, 200, 150), dialogueText = "New Node" });
    }
}
