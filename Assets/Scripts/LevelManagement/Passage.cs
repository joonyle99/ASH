using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : MonoBehaviour, ITriggerZone
{
    [SerializeField] PassageData _data;
    [SerializeField] PassageData _otherPassageData;

    [SerializeField] InputSetterScriptableObject _enterInputSetter;
    [SerializeField] InputSetterScriptableObject _exitInputSetter;

    public PassageData Data { get { return _data; } set { _data = value; } }

    public void OnActivatorEnter(TriggerActivator activator)
    {
        SceneTransitionManager.Instance.StartSceneChange(_otherPassageData.TargetSceneName);
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
    }

    public void OnExitPassaage()
    {
        InputManager.Instance.ChangeInputSetter(_exitInputSetter);
    }


}


#if UNITY_EDITOR
[CustomEditor(typeof(Passage), true), CanEditMultipleObjects]
public class PassageEditor : Editor
{
    static string RecentDirectoryPath = "";
    public override void OnInspectorGUI()
    {
        Passage passage = (Passage)target;
        if(passage.Data == null)
        {
            if (GUILayout.Button("Generate Passage Data"))
            {
                UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.GetActiveScene();
                string outputPath = EditorUtility.SaveFilePanel("Choose directory", RecentDirectoryPath, 
                                     currentScene.name + "_" + passage.name, "asset");
                RecentDirectoryPath = outputPath;
                outputPath = outputPath.Substring(outputPath.IndexOf("Assets/"));

                System.Type type = typeof(PassageData);
                ScriptableObject asset = CreateInstance(type);
                FieldInfo targetScene = type.GetField("_targetScene", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo name = type.GetField("_name", BindingFlags.Instance | BindingFlags.NonPublic);

                Tymski.SceneReference sceneRef = new Tymski.SceneReference();
                sceneRef.ScenePath  = currentScene.path;
                targetScene.SetValue(asset, sceneRef);
                name.SetValue(asset, passage.name);


                AssetDatabase.CreateAsset(asset, outputPath);
                AssetDatabase.SaveAssets();
                passage.Data = asset as PassageData;
            }
        }
        DrawDefaultInspector();
        Repaint();
    }
}
#endif