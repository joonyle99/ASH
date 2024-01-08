using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public class PolygonColliderGenerator : EditorWindow
{
    PolygonCollider2D _collider;
    int _corners;
    float _radius;
    bool _realTime = false;
    [MenuItem("Window/Utilities/Polygon Collider Generator")]
    static void Init()
    {
        GetWindow(typeof(PolygonColliderGenerator));
    }
    public void OnGUI()
    {
        _collider = EditorGUILayout.ObjectField("Collider", _collider, typeof(PolygonCollider2D), true) as PolygonCollider2D;
        _corners = EditorGUILayout.IntField("Corners", _corners);
        _radius = EditorGUILayout.FloatField("Radius", _radius);
        _realTime = EditorGUILayout.Toggle("Apply real time", _realTime);
        if (GUILayout.Button("Generate") || _realTime)
        {
            var points = new Vector2[_corners];
            float angleSlice = Mathf.PI * 2 / _corners;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Vector2(Mathf.Cos(i * angleSlice), Mathf.Sin(i * angleSlice)) * _radius;
            }
            _collider.points = points;
        }
    }
}