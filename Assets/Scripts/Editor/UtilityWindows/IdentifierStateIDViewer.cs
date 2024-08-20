using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdentifierStateIDViewer : EditorWindow
{
    // 윈도우를 열기 위한 메뉴
    [MenuItem("Window/Identifier StateID Viewer")]
    static void Init()
    {
        // 윈도우를 열기
        GetWindow(typeof(IdentifierStateIDViewer));
    }

    public void OnGUI()
    {
        // 현재 씬의 모든 오브젝트를 가져온다
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var allRootObjects = currentScene.GetRootGameObjects();

        // 모든 오브젝트에서 PreserveState 컴포넌트를 가져온다
        List<Identifier> allPreserveStateComponents = new List<Identifier>();
        foreach (var rootObject in allRootObjects)
        {
            allPreserveStateComponents.AddRange(rootObject.GetComponentsInChildren<Identifier>());
        }

        // 오브젝트를 이름순으로 정렬한다
        // allPreserveStateComponents.Sort(new PreserveStateComparer());

        // PreserveState 컴포넌트를 보여준다
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object");
        EditorGUILayout.LabelField("GroupName");
        EditorGUILayout.LabelField("ID");
        EditorGUILayout.EndHorizontal();

        // PreserveState 컴포넌트를 보여준다
        foreach (var preserveStateComponent in allPreserveStateComponents)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;

            // 오브젝트를 보여준다
            EditorGUILayout.ObjectField("", preserveStateComponent, typeof(Identifier), true);

            GUI.enabled = true;

            // 그룹 이름과 ID를 보여준다
            string groupName = EditorGUILayout.TextField(preserveStateComponent.EditorGroupName);
            string id = EditorGUILayout.TextField(preserveStateComponent.EditorID);

            // 그룹 이름과 ID가 변경되었다면 저장한다
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

    // IdentifierState를 그룹 이름으로 정렬하기 위한 비교자
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
