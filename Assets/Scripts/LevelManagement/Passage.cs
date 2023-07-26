using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Passage : ITriggerZone
{
    [SerializeField] PassageData _data;
    [SerializeField] PassageData _otherPassageData;

    [Tooltip("플레이어가 여기로 들어가서 다음 스테이지로 갈 때")][SerializeField] InputSetterScriptableObject _enterInputSetter;
    [Tooltip("플레이어가 이전 스테이지에서 여기로 나올 때")][SerializeField] InputSetterScriptableObject _exitInputSetter;

    [SerializeField] Transform _playerSpawnPoint;

    public PassageData Data { get { return _data; } set { _data = value; } }

    bool _isPlayerExiting;
    void Awake()
    {
        if (_playerSpawnPoint == null)
            _playerSpawnPoint = transform;
    }
    public override void OnActivatorEnter(TriggerActivator activator)
    {
        if (_isPlayerExiting)
            return;
        SceneManager.Instance.StartSceneChangeByPassage(_otherPassageData);
        InputManager.Instance.ChangeInputSetter(_enterInputSetter);
    }
    public override void OnActivatorExit(TriggerActivator activator)
    {
        if (!_isPlayerExiting)
            return;
        if (activator.IsPlayer)
            _isPlayerExiting = false;
    }
    
    //Passage를 통해 밖으로 나옴
    public IEnumerator PlayerExitCoroutine(PlayerBehaviour player)
    {
        //Spawn player
        _isPlayerExiting = true;
        Camera.main.GetComponent<CameraController>().SnapFollow();
        player.transform.position = _playerSpawnPoint.position;
        if (_exitInputSetter == null)
            InputManager.Instance.ChangeToDefaultSetter();
        else
            InputManager.Instance.ChangeInputSetter(_exitInputSetter);
        yield return null;

        //Wait until player exits zone
        yield return new WaitUntil(() => !_isPlayerExiting);
        yield return new WaitForSeconds(0.3f);
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