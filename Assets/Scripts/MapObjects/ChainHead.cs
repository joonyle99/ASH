using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChainHead : InteractableObject
{ 
    [SerializeField] WaypointPath _rail;
    float _chainLength { get { return _pieceCount * _pieceOffset; } }

    [Range(2, 10)][SerializeField] int _pieceCount;
    [SerializeField] float _pieceOffset;

    [SerializeField] ChainHandle _handle;
    [SerializeField] Rigidbody2D _firstChainPiece;
    [SerializeField] Joint2D _midChainPiece;
    [SerializeField] Joint2D _lastChainPiece;
    [SerializeField] DistanceJoint2D _playerLimittingJoint;


    protected override void OnInteract()
    {
        _handle.ConnectTo(Player.HandRigidBody);
        //_jointWithPlayer.enabled = true;
        Player.MovementController.enabled = true;
        _playerLimittingJoint.connectedBody = Player.Rigidbody;
        _playerLimittingJoint.connectedAnchor = Player.HandRigidBody.position - Player.Rigidbody.position;
        _playerLimittingJoint.distance = _chainLength;
    }
    protected override void OnInteractionExit()
    {
        _handle.Disconnect();
        _playerLimittingJoint.enabled = false;
        Player.MovementController.EnableMovementExternaly(this);
    }
    public override void UpdateInteracting()
    {
        if (IsInteractionKeyUp)
            ExitInteraction();
    }
    public override void FixedUpdateInteracting()
    {
        Vector3 playerPos = SceneContext.Current.Player.transform.position;
        if (Vector3.Distance(transform.position, playerPos) > _chainLength)
        {
            Intersection intersection = new Intersection();
            for (int i = 0; i < _rail.Count - 1; i++)
            {
                var result = IntersectionWithPlayer(_rail[i].position, _rail[i + 1].position, playerPos);
                if (result.Intersects)
                {
                    if (!intersection.Intersects)
                        intersection = result;
                    else if (Vector3.SqrMagnitude(transform.position - intersection.Point) > Vector3.SqrMagnitude(transform.position - result.Point))
                    {
                        intersection.Point = result.Point;
                    }
                }
            }
            print(intersection.Point);
            if (intersection.Intersects)
            {
                transform.position = intersection.Point;
                _playerLimittingJoint.enabled = false;
            }
            else
            {
                if (Player.RawInputs.Movement.x >= 0 && transform.position.x < Player.transform.position.x
                    || Player.RawInputs.Movement.x <= 0 && transform.position.x > Player.transform.position.x)
                    Player.MovementController.DisableMovementExternaly(this);
                else
                    Player.MovementController.EnableMovementExternaly(this);
                _playerLimittingJoint.enabled = true;
                print(Vector3.Distance(Player.HandRigidBody.position, transform.position));
            }
        }
        else
        {
            _playerLimittingJoint.enabled = false;

            Player.MovementController.EnableMovementExternaly(this);
        }
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
        //_lastChainPiece.transform.SetAsLastSibling();

    }
    struct Intersection
    {
        public Vector3 Point;
        public bool Intersects;
    }
    Intersection IntersectionWithPlayer(Vector3 p0, Vector3 p1, Vector3 playerPos)
    {
        Intersection result = new Intersection();
        Vector3 m = p1 - p0;
        Vector3 k = p0 - playerPos;
        float a = m.x * m.x + m.y * m.y;
        float b = 2 * (k.x * m.x + k.y * m.y);
        float c = k.x * k.x + k.y * k.y - _chainLength * _chainLength;

        float D = b * b - 4 * a * c;
        if (D < 0)
            result.Intersects = false;
        else
        {
            float t1 = (-b - Mathf.Sqrt(D)) / (2 * a);
            float t2 = (-b + Mathf.Sqrt(D)) / (2 * a);
            if (0 <= t1 && t1 <= 1)
            {
                result.Point = m * t1 + p0;
                result.Intersects = true;
            }
            if (0 <= t2 && t2 <= 1)
            {
                Vector3 newIntersection = m * t2 + p0;
                //TODO : Distance on the rail
                if (!result.Intersects || Vector3.SqrMagnitude(transform.position - result.Point) > Vector3.SqrMagnitude(transform.position - newIntersection))
                    result.Point = newIntersection;
                result.Intersects = true;
            }
        }
        return result;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _chainLength);
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(ChainHead))]
public class ChainHeadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChainHead t = (ChainHead)target;
        if (GUILayout.Button("Submit"))
            t.Submit();
    }
}
#endif