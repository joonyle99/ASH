using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ParallaxBoundaries
{
    [SerializeField] [HideInInspector] float [] _minBoundaries;
    [SerializeField] [HideInInspector] string [] _layerNames;
    [SerializeField] [HideInInspector] bool [] _enables;
    [SerializeField] [HideInInspector] int _lastEnabled = -1;


    public ParallaxBoundaries()
    {
    }
    public List<float> GetEnabledMinBoundaries()
    {
        List<float> result = new List<float>();
        for (int i = 0; i < _layerNames.Length; i++)
        {
            if (_enables[i])
                result.Add(_minBoundaries[i]);
        }
        return result;
    }
    public List<string> GetEnabledLayerNames()
    {
        List<string> result = new List<string>();
        for (int i = 0; i < _layerNames.Length; i++)
        {
            if (_enables[i])
                result.Add(_layerNames[i]);
        }
        return result;
    }
    public void SetSortingLayers(SortingLayer [] layers)
    {
        //TODO : 이전 값이 있으면 유지하는 기능 필요
        _layerNames = new string[layers.Length];
        _minBoundaries = new float[layers.Length];
        _enables = new bool[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            _layerNames[i] = layers[i].name;
            _enables[i] = true;
        }
        _lastEnabled = layers.Length - 1;
    }
    public string GetLayerName(float z)
    {
        for(int i=0; i<_layerNames.Length; i++)
        {
            if (!_enables[i])
                continue;
            if (_minBoundaries[i] <= z)
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
    SerializedProperty _minBoundaries;
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
                _minBoundaries = property.FindPropertyRelative("_minBoundaries");
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
                EditorGUI.PropertyField(fieldRect, _minBoundaries.GetArrayElementAtIndex(i), GUIContent.none);
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