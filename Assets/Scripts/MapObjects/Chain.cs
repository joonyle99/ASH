using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Chain : InteractableObject
{
    [Range(2,10)][SerializeField] int _pieceCount;
    [SerializeField] float _pieceOffset;

    [SerializeField] Rail _rail;
    [SerializeField] ChainHandle _handle;
    [SerializeField] DistanceJoint2D _jointWithPlayer;
    [SerializeField] Rigidbody2D _firstChainPiece;
    [SerializeField] Joint2D _midChainPiece;
    [SerializeField] Joint2D _lastChainPiece;

    [SerializeField] Rigidbody2D[] _chainBodies;

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
    Vector3 AngleToVector(float degree)
    {
        return new Vector3(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad), 0);
    }
    //Only In Editor
    public void Submit()
    {
        foreach (var child in transform.GetComponentsInChildren<Rigidbody2D>())
        {
            if (child.transform != transform && child.transform != _firstChainPiece.transform &&
                child.transform != _midChainPiece.transform && child.transform != _lastChainPiece.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        var upperPiece = _firstChainPiece;
        for (int i = 0; i < _pieceCount - 2; i++)
        {
            var newPiece = Instantiate(_midChainPiece, transform);
            newPiece.gameObject.SetActive(true);
            newPiece.transform.position = upperPiece.transform.position + _pieceOffset * AngleToVector(upperPiece.transform.rotation.eulerAngles.z);
            newPiece.connectedBody = upperPiece;
            upperPiece = newPiece.attachedRigidbody;
        }
        _lastChainPiece.transform.position = upperPiece.transform.position + _pieceOffset * AngleToVector(upperPiece.transform.rotation.eulerAngles.z);
        _lastChainPiece.connectedBody = upperPiece;
        _lastChainPiece.transform.SetAsLastSibling();

        _chainBodies = GetComponentsInChildren<Rigidbody2D>();
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
        if (GUILayout.Button("Submit"))
            t.Submit();
    }
}
#endif