using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreserveStateIDViewer : EditorWindow
{
    private static Scene _currentScene;
    private static List<PreserveState> _allPreserveStateComponents;
    private static GameObject[] _allRootObjects;

    // 윈도우를 열기 위한 메뉴
    [MenuItem("Window/Preserve StateID Viewer")]
    static void Init()
    {
        // 현재 씬의 모든 오브젝트를 가져온다
        _currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        // 모든 오브젝트에서 PreserveState 컴포넌트를 가져온다
        _allPreserveStateComponents = new List<PreserveState>();
        _allRootObjects = _currentScene.GetRootGameObjects();

        foreach (var rootObject in _allRootObjects)
        {
            _allPreserveStateComponents.AddRange(rootObject.GetComponentsInChildren<PreserveState>());
        }

        //오브젝트를 id순으로 정렬한다, 이 때 빈 id가 있다면 제일 아래 배치
        _allPreserveStateComponents.Sort(new PreserveStateComparer().CompareWithId);

        // 윈도우를 열기
        GetWindow(typeof(PreserveStateIDViewer));

    }

    public void OnGUI()
    {
        if (_allPreserveStateComponents == null || _currentScene == null)
            return;

        // 오브젝트를 이름순으로 정렬한다
        // allPreserveStateComponents.Sort(new PreserveStateComparer());

        // PreserveState 컴포넌트를 보여준다
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object");
        EditorGUILayout.LabelField("GroupName");
        EditorGUILayout.LabelField("ID");
        EditorGUILayout.EndHorizontal();

        // PreserveState 컴포넌트를 보여준다
        foreach (var preserveStateComponent in  _allPreserveStateComponents)
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = false;

            // 오브젝트를 보여준다
            EditorGUILayout.ObjectField("", preserveStateComponent, typeof(PreserveState), true);

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
                EditorSceneManager.MarkSceneDirty(_currentScene);
                //EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    // PreserveState를 그룹 이름으로 정렬하기 위한 비교자
    public class PreserveStateComparer : IComparer<PreserveState>
    {
        public int Compare(PreserveState x, PreserveState y)
        {
            if (x.EditorGroupName.CompareTo(y.EditorGroupName) < 0)
                return -1;

            return 1;
        }

        public int CompareWithId(PreserveState x, PreserveState y)
        {
            if (x.EditorID.Length <= 2) return 1;
            if (y.EditorID.Length <= 2) return -1;

            int xId = int.Parse(x.EditorID.Substring(2, x.EditorID.Length - 2));
            int yId = int.Parse(y.EditorID.Substring(2, y.EditorID.Length - 2));

            if (xId.CompareTo(yId) < 0)
                return -1;

            return 1;
        }
    }
}