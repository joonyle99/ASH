using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public class PolygonColliderScaler : EditorWindow
{
    PolygonCollider2D _collider;
    Vector2 _scale;
    [MenuItem("Window/Utilities/Polygon Collider Scaler")]
    static void Init()
    {
        GetWindow(typeof(PolygonColliderScaler));
    }
    public void OnGUI()
    {
        _collider = EditorGUILayout.ObjectField("Collider", _collider, typeof(PolygonCollider2D), true) as PolygonCollider2D;
        _scale = EditorGUILayout.Vector2Field("Scale", _scale);
        if (GUILayout.Button("Scale"))
        {
            var points = _collider.points;
            for(int i=0; i< points.Length; i++)
            {
                points[i].Scale(_scale);
            }
            _collider.points = points;
        }
    }
}