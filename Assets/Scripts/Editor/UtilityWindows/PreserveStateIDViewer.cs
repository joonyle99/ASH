using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreserveStateIDViewer : EditorWindow
{
    // �����츦 ���� ���� �޴�
    [MenuItem("Window/Preserve StateID Viewer")]
    static void Init()
    {
        // �����츦 ����
        GetWindow(typeof(PreserveStateIDViewer));
    }

    public void OnGUI()
    {
        // ���� ���� ��� ������Ʈ�� �����´�
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var allRootObjects = currentScene.GetRootGameObjects();
        
        // ��� ������Ʈ���� PreserveState ������Ʈ�� �����´�
        List<PreserveState> allPreserveStateComponents = new List<PreserveState>();
        foreach(var rootObject in allRootObjects )
        {
            allPreserveStateComponents.AddRange(rootObject.GetComponentsInChildren<PreserveState>());
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
        foreach (var preserveStateComponent in  allPreserveStateComponents )
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;

            // ������Ʈ�� �����ش�
            EditorGUILayout.ObjectField("", preserveStateComponent, typeof(PreserveState), true);

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

    // PreserveState�� �׷� �̸����� �����ϱ� ���� ����
    public class PreserveStateComparer : IComparer<PreserveState>
    {
        public int Compare(PreserveState x, PreserveState y)
        {
            if (x.EditorGroupName.CompareTo(y.EditorGroupName) < 0)
                return -1;

            return 1;
        }
    }
}