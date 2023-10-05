using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using System.Drawing.Printing;
using Com.LuisPedroFonseca.ProCamera2D;
using System.ComponentModel;
using System;

namespace LevelGraph
{
    public class LevelGraphEditor : EditorWindow
    {
        LevelGraphView _levelGraphView;
        ObjectField _levelGraphAssetField;

        [MenuItem("Window/Level Graph Editor")]
        public static void ShowWindow()
        {
            LevelGraphEditor wnd = GetWindow<LevelGraphEditor>();
            wnd.titleContent = new GUIContent("LevelGraphEditor");
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/LevelGraphEditor/LevelGraphEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/LevelGraphEditor/LevelGraphEditor.uss");

            _levelGraphView = rootVisualElement.Query<LevelGraphView>().AtIndex(0);
            _levelGraphView.SetNodeInspector(rootVisualElement.Query<VisualElement>(name: "NodeInspector"));

            _levelGraphAssetField = rootVisualElement.Query<ObjectField>(name: "LevelGraphAsset");
            _levelGraphAssetField.RegisterValueChangedCallback(OnLevelGraphAssetChanged);
        }

        private void OnLevelGraphAssetChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            LevelGraphData graphData = rootVisualElement.Query<ObjectField>().AtIndex(0).value as LevelGraphData;
            if (graphData != null)
            {
                SerializedObject so = new SerializedObject(graphData);
                rootVisualElement.Bind(so);
                if (_levelGraphView != null)
                    _levelGraphView.PopulateView(graphData);
            }
            else
            {
                rootVisualElement.Unbind();
                _levelGraphView.ClearView();
                
            }
        }

    }
}