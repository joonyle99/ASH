using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//https://docs.unity3d.com/ScriptReference/SceneAsset.html

[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
public class ScenePickerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        property.serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newScene = EditorGUI.ObjectField(position, property.displayName, oldScene, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPath = AssetDatabase.GetAssetPath(newScene);
            property.stringValue = newPath;
        }
        property.serializedObject.ApplyModifiedProperties();
    }
}
