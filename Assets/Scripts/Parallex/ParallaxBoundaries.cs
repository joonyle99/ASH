using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ParallaxBoundaries
{
    [SerializeField] [HideInInspector] float [] _boundaries;
    [SerializeField] [HideInInspector] string []_layerNames;
    [SerializeField] [HideInInspector] bool [] _enables;
    [SerializeField] [HideInInspector] int _lastEnabled = -1;


    public float[] Boundaries { get { return _boundaries; } }
    public ParallaxBoundaries()
    {
    }
    
    public void SetSortingLayers(SortingLayer [] layers)
    {
        _layerNames = new string[layers.Length];
        _boundaries = new float[layers.Length];
        _enables = new bool[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            _layerNames[i] = layers[i].name;
            _enables[i] = true;
        }
    }
    public string GetLayerName(float z)
    {
        for(int i=0; i<_layerNames.Length; i++)
        {
            if (!_enables[i])
                continue;
            if (_boundaries[i] <= z)
                return _layerNames[i];
        }
        return _layerNames[_lastEnabled];
    }
    public void OnValidate()
    {
        int cnt = 0;
        for (int i = 0; i < _enables.Length; i++)
        {
            if (!_enables[i])
                continue;
            cnt++;
            _lastEnabled = i;
        }
        if (cnt < 2)
           _lastEnabled = -1;
    }
}



#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ParallaxBoundaries))]
public class ParallaxBoundariesDrawer : PropertyDrawer
{
    int HEIGHT = 18;

    SerializedProperty _layerNames;
    SerializedProperty _boundaries;
    SerializedProperty _enables;
    SerializedProperty _lastEnabled;
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
            if(_layerNames == null)
            {
                _layerNames = property.FindPropertyRelative("_layerNames");
                _boundaries = property.FindPropertyRelative("_boundaries");
                _enables = property.FindPropertyRelative("_enables");
                _lastEnabled = property.FindPropertyRelative("_lastEnabled");
            }
            
            for (int i=0; i<_layerNames.arraySize; i++)
            {
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, HEIGHT), _enables.GetArrayElementAtIndex(i), new GUIContent(""));
                
                Rect afterEnabled = EditorGUI.IndentedRect(new Rect(position.x + 4, position.y, position.width, HEIGHT));
                Rect contentRect = EditorGUI.PrefixLabel(afterEnabled, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(_layerNames.GetArrayElementAtIndex(i).stringValue));
                Rect fieldRect = EditorGUI.IndentedRect(new Rect(contentRect.x, contentRect.y, contentRect.width, HEIGHT));
                GUI.enabled = _enables.GetArrayElementAtIndex(i).boolValue;
                if (_lastEnabled.intValue == i)
                    GUI.enabled = false;
                EditorGUI.PropertyField(fieldRect, _boundaries.GetArrayElementAtIndex(i), GUIContent.none);
                GUI.enabled = true;
                position.y += HEIGHT + 2;
            }
        }
        
        EditorGUI.indentLevel = indent;
        EditorGUI.EndFoldoutHeaderGroup();
        if (_lastEnabled.intValue == -1)
        {
            EditorGUI.HelpBox(new Rect(position.x, position.y, position.width, HEIGHT*2), "Need more than 2 enabled layers!", MessageType.Error);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int total = 0;
        if (_lastEnabled != null && _lastEnabled.intValue == -1)
            total += 2 * HEIGHT;

        if (!property.isExpanded)
            total+= HEIGHT + 2;
        else
            total += HEIGHT * (property.FindPropertyRelative("_layerNames").arraySize + 2) + 2;
        return total;
    }
}
#endif