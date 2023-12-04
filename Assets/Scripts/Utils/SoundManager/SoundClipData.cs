using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SoundClipData
{
    public enum SelectionType
    {
        Single, RandomRange
    }

    public string Key { get { return _key; } }
    public AudioClip Clip { get { return _clip; } }

    public float Volume
    {
        get
        {
            if (_volumeType == SelectionType.Single)
                return _volumeA;
            else
                return Random.Range(_volumeA, _volumeB);
        }
    }
    public float Pitch
    {
        get
        {
            if (_pitchType == SelectionType.Single)
                return _pitchA;
            else
                return Random.Range(_pitchA, _pitchB);
        }
    }

    [SerializeField][HideInInspector] string _key;
    [SerializeField][HideInInspector] AudioClip _clip;
    [SerializeField][HideInInspector] SelectionType _volumeType;
    [SerializeField][HideInInspector] float _volumeA = 1;
    [SerializeField][HideInInspector] float _volumeB = 1;
    [SerializeField][HideInInspector] SelectionType _pitchType;
    [SerializeField][HideInInspector] float _pitchA = 1;
    [SerializeField][HideInInspector] float _pitchB = 1;

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SoundClipData))]
public class SoundClipDataDrawer : PropertyDrawer
{
    int HEIGHT = 18;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;

        Rect foldoutRect = EditorGUI.IndentedRect(new Rect(position.x, position.y, position.width, HEIGHT));
        position.y += HEIGHT;
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            position = DrawProperty(position, "Key", "_key", property);
            position = DrawProperty(position, "Clip", "_clip", property);

            //volume
            {
                var volumeTypeProperty = property.FindPropertyRelative("_volumeType");
                Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Volume Type"));
                Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
                EditorGUI.PropertyField(fieldRect, volumeTypeProperty, GUIContent.none);
                position.y += HEIGHT + 2;

                if ((SoundClipData.SelectionType)volumeTypeProperty.enumValueIndex == SoundClipData.SelectionType.Single)
                {
                    position = DrawProperty(position, "Volume", "_volumeA", property);
                }
                else
                {
                    contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Volume Range"));

                    Rect fieldRectA = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width/2, HEIGHT));
                    Rect fieldRectB = EditorGUI.IndentedRect(new Rect(fieldRectA.xMax, contentRect.y, contentRect.width/2, HEIGHT));
                    EditorGUI.PropertyField(fieldRectA, property.FindPropertyRelative("_volumeA"), GUIContent.none);
                    EditorGUI.LabelField(new Rect(fieldRectA.xMax- 5, contentRect.y, contentRect.width / 2, HEIGHT), "~");
                    EditorGUI.PropertyField(fieldRectB, property.FindPropertyRelative("_volumeB"), GUIContent.none);
                    position.y += HEIGHT + 2;
                }
            }
            //pitch
            {
                var pitchTypeProperty = property.FindPropertyRelative("_pitchType");
                Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Pitch Type"));
                Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
                EditorGUI.PropertyField(fieldRect, pitchTypeProperty, GUIContent.none);
                position.y += HEIGHT + 2;

                if ((SoundClipData.SelectionType)pitchTypeProperty.enumValueIndex == SoundClipData.SelectionType.Single)
                {
                    position = DrawProperty(position, "Pitch", "_pitchA", property);
                }
                else
                {
                    contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Pitch Range"));

                    Rect fieldRectA = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width / 2, HEIGHT));
                    Rect fieldRectB = EditorGUI.IndentedRect(new Rect(fieldRectA.xMax, contentRect.y, contentRect.width / 2, HEIGHT));
                    EditorGUI.PropertyField(fieldRectA, property.FindPropertyRelative("_pitchA"), GUIContent.none);
                    EditorGUI.LabelField(new Rect(fieldRectA.xMax - 5, contentRect.y, contentRect.width / 2, HEIGHT), "~");
                    EditorGUI.PropertyField(fieldRectB, property.FindPropertyRelative("_pitchB"), GUIContent.none);
                    position.y += HEIGHT + 2;
                }
            }

        }
        EditorGUI.indentLevel = indent;
        EditorGUI.EndFoldoutHeaderGroup();
        EditorGUI.EndProperty();
    }
    Rect DrawProperty(Rect position, string label, string propertyName, SerializedProperty property)
    {
        Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label));
        Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(propertyName), GUIContent.none);
        position.y += HEIGHT + 2;
        return position;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return (HEIGHT + 2) * 7;
        }
        else
            return (HEIGHT + 2);
    }
}
#endif