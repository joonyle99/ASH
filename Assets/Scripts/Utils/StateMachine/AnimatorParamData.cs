using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class AnimatorParamData 
{
    public enum ParamType
    {
        Float,
        Int,
        Bool,
        Trigger,
    }
    public ParamType Type { get { return _type; } }
    public string ParamName { get { return _paramName; } }

    [SerializeField][HideInInspector] ParamType _type;
    [SerializeField][HideInInspector] string _paramName;
    [SerializeField][HideInInspector] int _intValue;
    [SerializeField][HideInInspector] float _floatValue;
    [SerializeField][HideInInspector] bool _boolValue;

    public void Invoke(Animator animator)
    {
        switch (Type)
        {
            case ParamType.Float:
                animator.SetFloat(ParamName, _floatValue);
                break;
            case ParamType.Int:
                animator.SetInteger(ParamName, _intValue);
                break;
            case ParamType.Bool:
                animator.SetBool(ParamName, _boolValue);
                break;
            case ParamType.Trigger:
                animator.SetTrigger(ParamName);
                break;
        }
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AnimatorParamData))]
public class AnimatorParamDataDrawer : PropertyDrawer
{
    int HEIGHT = 18;
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

            Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Param Type"));
            Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
            EditorGUI.PropertyField(fieldRect, typeProperty, GUIContent.none);
            position.y += HEIGHT+2;

            contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Param Name"));
            fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("_paramName"), GUIContent.none);
            position.y += HEIGHT + 2;

            if ((AnimatorParamData.ParamType)typeProperty.enumValueIndex != AnimatorParamData.ParamType.Trigger)
            {
                contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Value"));
                fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
                switch ((AnimatorParamData.ParamType)typeProperty.enumValueIndex)
                {
                    case AnimatorParamData.ParamType.Float:
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("_floatValue"), GUIContent.none);
                        break;
                    case AnimatorParamData.ParamType.Int:
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("_intValue"), GUIContent.none);
                        break;
                    case AnimatorParamData.ParamType.Bool:
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("_boolValue"), GUIContent.none);
                        break;
                }
                position.y += HEIGHT + 2;
            }
        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.isExpanded)
        {
            if ((AnimatorParamData.ParamType)property.FindPropertyRelative("_type").enumValueIndex != AnimatorParamData.ParamType.Trigger)
                return (HEIGHT + 2) * 4;
            else
                return (HEIGHT + 2) * 3;
        }
        else
            return (HEIGHT + 2);
    }
}
#endif