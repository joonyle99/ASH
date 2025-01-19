#if UNITY_EDITOR
using static WaveSpawnMonsterInfo;
using System.Text.RegularExpressions;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(WaveSpawnMonsterInfo))]
public class WaveMonsterInfoDrawer : PropertyDrawer
{
    private int HEIGHT = 18;
    private UnityEventDrawer _eventDrawer;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;

        Rect foldoutRect = EditorGUI.IndentedRect(new Rect(position.x, position.y, position.width, HEIGHT));
        position.y += HEIGHT + 2;
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            var typeProperty = property.FindPropertyRelative("InstanceType");

            Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("MonsterInstanceType"));
            Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
            EditorGUI.PropertyField(fieldRect, typeProperty, GUIContent.none);
            position.y += HEIGHT + 2;
            DrawField("Monster", position, property);
            position.y += HEIGHT + 2;

            switch ((MonsterInstanceType)typeProperty.enumValueIndex)
            {
                case MonsterInstanceType.Prefab:
                    DrawField("Count", position, property);
                    position.y += HEIGHT + 2;
                    DrawField("SpawnPosition", position, property);
                    position.y += HEIGHT + 2;
                    break;
            }
        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    private void DrawField(string fieldName, Rect position, SerializedProperty property, string inspectorName = "")
    {
        if (inspectorName == "") inspectorName = SplitCamelCase(fieldName);
        Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(inspectorName));
        Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(fieldName), GUIContent.none);
    }
    public static string SplitCamelCase(string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            var effectType = (MonsterInstanceType)property.FindPropertyRelative("InstanceType").enumValueIndex;

            if (effectType == MonsterInstanceType.Prefab)
                return (HEIGHT + 2) * 5;
            else
                return (HEIGHT + 2) * 3;
        }
        else
            return (HEIGHT + 2);
    }
}
#endif