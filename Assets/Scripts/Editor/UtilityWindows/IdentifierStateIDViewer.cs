using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdentifierStateIDViewer : EditorWindow
{
    // �����츦 ���� ���� �޴�
    [MenuItem("Window/Identifier StateID Viewer")]
    static void Init()
    {
        // �����츦 ����
        GetWindow(typeof(IdentifierStateIDViewer));
    }

    public void OnGUI()
    {
        // ���� ���� ��� ������Ʈ�� �����´�
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var allRootObjects = currentScene.GetRootGameObjects();

        // ��� ������Ʈ���� PreserveState ������Ʈ�� �����´�
        List<Identifier> allPreserveStateComponents = new List<Identifier>();
        foreach (var rootObject in allRootObjects)
        {
            allPreserveStateComponents.AddRange(rootObject.GetComponentsInChildren<Identifier>());
        }

        // ������Ʈ�� �̸������� �����Ѵ�
        // allPreserveStateComponents.Sort(new PreserveStateComparer());

        // PreserveState ������Ʈ�� �����ش�
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object");
        EditorGUILayout.LabelField("GroupName");
        EditorGUILayout.LabelField("ID");
        EditorGUILayout.EndHorizontal();

        // PreserveState ������Ʈ�� �����ش�
        foreach (var preserveStateComponent in allPreserveStateComponents)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;

            // ������Ʈ�� �����ش�
            EditorGUILayout.ObjectField("", preserveStateComponent, typeof(Identifier), true);

            GUI.enabled = true;

            // �׷� �̸��� ID�� �����ش�
            string groupName = EditorGUILayout.TextField(preserveStateComponent.EditorGroupName);
            string id = EditorGUILayout.TextField(preserveStateComponent.EditorID);

            // �׷� �̸��� ID�� ����Ǿ��ٸ� �����Ѵ�
            if (preserveStateComponent.EditorGroupName != groupName ||
                preserveStateComponent.EditorID != id)
            {
                preserveStateComponent.EditorGroupName = groupName;
                preserveStateComponent.EditorID = id;
                EditorSceneManager.MarkSceneDirty(currentScene);
                //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    // IdentifierState�� �׷� �̸����� �����ϱ� ���� ����
    public class IdentifierStateComparer : IComparer<Identifier>
    {
        public int Compare(Identifier x, Identifier y)
        {
            if (x.EditorGroupName.CompareTo(y.EditorGroupName) < 0)
                return -1;

            return 1;
        }
    }
}
