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

    [SerializeField] SoundList _soundList;
    [SerializeField] AudioSource _moveAudio;

    int _currentRailSection = 0;
    float _moveSoundTimer = 0f;

    private void Update()
    {
        Vector3 angleVector = (_rail[_currentRailSection + 1].position - _rail[_currentRailSection].position);
        float angle = Mathf.Atan2(angleVector.y, angleVector.x);
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        _moveSoundTimer -= Time.deltaTime;
        if (_moveSoundTimer <= 0f)
        {
            _moveSoundTimer = 0f;
            _moveAudio.Stop();
        }
    }
    protected override void OnObjectInteractionEnter()
    {
        _soundList.PlaySFX("Grab");
        _handle.ConnectTo(Player.HandRigidBody);
        //_jointWithPlayer.enabled = true;
        Player.PlayerMovementController.enabled = true;
        _playerLimittingJoint.connectedBody = Player.Rigidbody;
        _playerLimittingJoint.connectedAnchor = Player.HandRigidBody.position - Player.Rigidbody.position;
        _playerLimittingJoint.distance = _chainLength;
    }
    protected override void OnObjectInteractionExit()
    {
        _handle.Disconnect();
        _playerLimittingJoint.enabled = false;
        Player.PlayerMovementController.EnableMovementExternal(this);
    }
    public override void UpdateInteracting()
    {
        if (IsInteractionKeyUp)
            ExitInteraction();
    }
    public override void FixedUpdateInteracting()
    {
        // 플레이어의 위치를 가져옴
        Vector3 playerPos = SceneContext.Current.Player.transform.position;

        // 객체와 플레이어 사이의 거리가 _chainLength보다 큰지 확인
        if (Vector3.Distance(transform.position, playerPos) > _chainLength)
        {
            // 교차점을 저장할 객체 생성
            Intersection intersection = new Intersection();

            // _rail 리스트의 각 구간을 순회하며 교차점 찾기
            for (int i = 0; i < _rail.Count - 1; i++)
            {
                var result = IntersectionWithPlayer(_rail[i].position, _rail[i + 1].position, playerPos);
                if (result.isIntersects)
                {
                    if (!intersection.isIntersects)
                    {
                        intersection = result;
                        _currentRailSection = i;
                    }
                    else if (Vector3.SqrMagnitude(transform.position - intersection.Point) > Vector3.SqrMagnitude(transform.position - result.Point))
                    {
                        intersection.Point = result.Point;
                        _currentRailSection = i;
                    }
                }
            }

            // 교차점이 있으면 객체의 위치를 교차점으로 이동
            if (intersection.isIntersects)
            {
                transform.position = intersection.Point;
                _playerLimittingJoint.enabled = false;
                if (!_moveAudio.isPlaying)
                    _moveAudio.Play();
                _moveSoundTimer = 1.7f;
            }
            // 교차점이 없으면 플레이어의 이동을 제한하거나 허용
            else
            {
                if (Player.RawInputs.Movement.x >= 0 && transform.position.x < Player.transform.position.x
                    || Player.RawInputs.Movement.x <= 0 && transform.position.x > Player.transform.position.x)
                    Player.PlayerMovementController.DisableMovementExternal(this);
                else
                    Player.PlayerMovementController.EnableMovementExternal(this);
                _playerLimittingJoint.enabled = true;
            }
        }
        // 객체와 플레이어 사이의 거리가 _chainLength 이하인 경우
        else
        {
            _playerLimittingJoint.enabled = false;

            Player.PlayerMovementController.EnableMovementExternal(this);
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
        public bool isIntersects;
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
            result.isIntersects = false;
        else
        {
            float t1 = (-b - Mathf.Sqrt(D)) / (2 * a);
            float t2 = (-b + Mathf.Sqrt(D)) / (2 * a);
            if (0 <= t1 && t1 <= 1)
            {
                result.Point = m * t1 + p0;
                result.isIntersects = true;
            }
            if (0 <= t2 && t2 <= 1)
            {
                Vector3 newIntersection = m * t2 + p0;
                //TODO : Distance on the rail
                if (!result.isIntersects || Vector3.SqrMagnitude(transform.position - result.Point) > Vector3.SqrMagnitude(transform.position - newIntersection))
                    result.Point = newIntersection;
                result.isIntersects = true;
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