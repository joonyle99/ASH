using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Chain : MonoBehaviour
{
    [SerializeField] HingeJoint2D _firstChainPiece;
    [SerializeField] float _pieceOffset;



    
    //Only In Editor
    HingeJoint2D GetLastPiece()
    {
        var joints = GetComponentsInChildren<HingeJoint2D>();
        return joints[joints.Length - 1];
    }
    //Only In Editor
    public void AddChain()
    {
        var lastPiece = GetLastPiece();
        var newPiece = Instantiate(_firstChainPiece, transform);
        newPiece.transform.position = lastPiece.transform.position + _pieceOffset * AngleToVector(lastPiece.transform.rotation.eulerAngles.z);
        newPiece.connectedBody = lastPiece.attachedRigidbody;
    }
    Vector3 AngleToVector(float degree)
    {
        return new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0);
    }
    //Only In Editor
    public void RemoveChain()
    {
        DestroyImmediate(GetLastPiece().gameObject);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Chain))]
public class ChainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Chain t = (Chain)target;
        if (GUILayout.Button("+1"))
        {
            t.AddChain();
        }
        if (GUILayout.Button("-1"))
        {
            t.RemoveChain();
        }
    }
}
#endif