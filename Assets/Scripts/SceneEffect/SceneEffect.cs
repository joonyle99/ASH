using UnityEngine;

using UnityEngine.Events;
using System.Text.RegularExpressions;
using Com.LuisPedroFonseca.ProCamera2D;

#if UNITY_EDITOR
// 커스텀 에디터를 사용하기 위한 네임스페이스
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class SceneEffect
{
    public enum EffectType
    {
        Dialogue,
        CameraShake,
        ConstantCameraShake,
        StopConstantCameraShake,
        WaitForSeconds,
        ChangeInputSetter,
        ChangeToDefaultInputSetter,
        FunctionCall,
        LifePurchase,
    }
    public EffectType Type => _type;
    public bool IsCameraEffect => _type == EffectType.CameraShake || _type == EffectType.ConstantCameraShake || _type == EffectType.StopConstantCameraShake;

    [SerializeField][HideInInspector] private EffectType _type = EffectType.Dialogue;
    [SerializeField][HideInInspector] public DialogueData DialogueData = null;
    [SerializeField][HideInInspector] public ShakePreset ShakeData = null;
    [SerializeField][HideInInspector] public ConstantShakePreset ConstantShakeData = null;
    [SerializeField][HideInInspector] public float Time = 0f;
    [SerializeField][HideInInspector] public InputSetterScriptableObject InputSetter = null;
    [SerializeField][HideInInspector] public UnityEvent Function;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneEffect))]
public class SceneEffectDrawer : PropertyDrawer
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
            var typeProperty = property.FindPropertyRelative("_type");

            Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Effect Type"));
            Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
            EditorGUI.PropertyField(fieldRect, typeProperty, GUIContent.none);
            position.y += HEIGHT + 2;

            switch((SceneEffect.EffectType)typeProperty.enumValueIndex)
            {
                case SceneEffect.EffectType.Dialogue:
                    DrawField("DialogueData", position, property);
                    break;
                case SceneEffect.EffectType.CameraShake:
                    DrawField("ShakeData", position, property);
                    break;
                case SceneEffect.EffectType.ConstantCameraShake:
                    DrawField("ConstantShakeData", position, property);
                    break;
                case SceneEffect.EffectType.StopConstantCameraShake:
                    DrawField("Time", position, property, "Smooth Time");
                    break;
                case SceneEffect.EffectType.ChangeInputSetter:
                    DrawField("InputSetter", position, property);
                    break;
                case SceneEffect.EffectType.ChangeToDefaultInputSetter:
                    position.y -= HEIGHT + 2;
                    break;
                case SceneEffect.EffectType.WaitForSeconds:
                    DrawField("Time", position, property);
                    break;
                case SceneEffect.EffectType.FunctionCall:
                    DrawField("Function", position, property);
                    break;
            }
            position.y += HEIGHT + 2;
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
    private float GetEventHeight(SerializedProperty property)
    {
        if (_eventDrawer == null)
            _eventDrawer = new UnityEventDrawer();
        return _eventDrawer.GetPropertyHeight(property, new GUIContent("Function"));
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            var effectType = (SceneEffect.EffectType)property.FindPropertyRelative("_type").enumValueIndex;
            if (effectType == SceneEffect.EffectType.ChangeToDefaultInputSetter ||
                effectType == SceneEffect.EffectType.LifePurchase)
                return (HEIGHT + 2) * 2;
            else if (effectType == SceneEffect.EffectType.FunctionCall)
                return (HEIGHT + 2) * 2 + GetEventHeight(property.FindPropertyRelative("Function"));
            else
                return (HEIGHT + 2) * 3;
        }
        else
            return (HEIGHT + 2);
    }
}
#endif