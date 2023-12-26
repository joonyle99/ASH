using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Chain : InteractableObject
{
    [SerializeField] Joint2D _firstChainPiece;
    [SerializeField] float _pieceOffset;

    [SerializeField] Rail _rail;
    [SerializeField] ChainHandle _handle;
    [SerializeField] DistanceJoint2D _jointWithPlayer;

    Rigidbody2D[] _chainBodies;

    private void Awake()
    {
        _chainBodies = GetComponentsInChildren<Rigidbody2D>();
    }

    protected override void OnInteract()
    {
        _handle.ConnectTo(Player.HandRigidBody);
        _jointWithPlayer.connectedBody = Player.HandRigidBody;
        //_jointWithPlayer.enabled = true;
        Player.MovementController.enabled = true;
    }
    public override void FixedUpdateInteracting()
    {
        for (int i = _chainBodies.Length - 2; i >= 0; i--)
        {
            if (Vector2.Distance(_chainBodies[i].position, _chainBodies[i + 1].position) > _pieceOffset * 0.9)
            {
                float dist = _pieceOffset * 0.8f;
                if (i == 1)
                    dist = 0;
                var dir = _chainBodies[i].position - _chainBodies[i + 1].position;
                _chainBodies[i].MovePosition(dir.normalized * dist + _chainBodies[i + 1].position);
            }
        }
        if (_rail.IsAtEndOfRail && Vector3.Distance(transform.position, Player.HandRigidBody.transform.position) >= 3.15f)
        {
            if (Player.RawInputs.Movement.x > 0 && transform.position.x < Player.transform.position.x)
                Player.DisableHorizontalMovement();
            else if (Player.RawInputs.Movement.x < 0 && transform.position.x > Player.transform.position.x)
                Player.DisableHorizontalMovement();
            else
                Player.EnableHorizontalMovement();
        }
        else
            Player.EnableHorizontalMovement();
    }
    protected override void OnInteractionExit()
    {
        _handle.Disconnect();
        _jointWithPlayer.enabled = false;
        Player.EnableHorizontalMovement();
    }
    public override void UpdateInteracting()
    {
        if (IsInteractionKeyUp)
            ExitInteraction();
    }

    //Only In Editor
    Joint2D GetLastPiece()
    {
        var joints = GetComponentsInChildren<Joint2D>();
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