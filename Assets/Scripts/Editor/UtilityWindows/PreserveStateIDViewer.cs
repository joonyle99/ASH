using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PreserveStateIDViewer : EditorWindow
{
    [MenuItem("Window/Preserve StateID Viewer")]
    static void Init()
    {
        GetWindow(typeof(PreserveStateIDViewer));
    }

    public void OnGUI()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        var allRootObjects = currentScene.GetRootGameObjects();
        
        List<PreserveState> allPreserveStateComponents = new List<PreserveState>();
        foreach(var rootObject in allRootObjects )
        {
            allPreserveStateComponents.AddRange(rootObject.GetComponentsInChildren<PreserveState>());
        }
        //allPreserveStateComponents.Sort(new PreserveStateComparer());

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Object");
        EditorGUILayout.LabelField("GroupName");
        EditorGUILayout.LabelField("ID");

        EditorGUILayout.EndHorizontal();
        foreach (var preserveStateComponent in  allPreserveStateComponents )
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("", preserveStateComponent, typeof(PreserveState), true);
            GUI.enabled = true;

            string groupName = EditorGUILayout.TextField(preserveStateComponent.EditorGroupName);
            string id = EditorGUILayout.TextField(preserveStateComponent.EditorID);

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