using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class LanternSceneContext : SceneContext
{
    [System.Serializable]
    class LanternRelation
    {
        public LanternLike A;
        public LanternLike B;
        [HideInInspector] public LightBeam Beam;
        [HideInInspector] public LightConnectionSparkEffect ConnectionSparkEffect;
        public bool IsConnected => Beam != null && Beam.gameObject.activeInHierarchy;
        public bool IsConnectionDone => IsConnected && Beam.IsShootingDone;
    }

    [Header("Lantern Scene Context")]
    [Space]

    [SerializeField] List<LanternRelation> _lanternRelations;

    [SerializeField] LightBeam _beamPrefab;
    [SerializeField] LayerMask _beamObstacleLayers;
    [SerializeField] LightConnectionSparkEffect _sparkEffectPrefab;
    [SerializeField] LightDoor _lightDoor;
    [SerializeField] float _silenceTimeOnStart = 2f;

    [Header("Last Connection Camera Effect")]
    [Space]

    [SerializeField] Transform _lastConnectionCameraPoint;
    [SerializeField] InputSetterScriptableObject _lastConnectionInputSetter;
    [SerializeField] float _cameraLastLanternStayDuration;
    [SerializeField] float _lastBeamDuration = 1;
    [SerializeField] float _cameraDoorStayDuration;
    [SerializeField] ShakePreset _beamHitDoorPreset;
    [SerializeField] ConstantShakePreset _beamShootingPreset;

    [SerializeField] SoundList _soundList;

    List<LanternLike> _lanternActivationOrder = new List<LanternLike>();

    const float MaxRayCastDistance = 1000f;
    const uint MaxRayCastHitCount = 5;

    bool _StopCheckingConnections = false;
    bool _isSilenced = true;

    public new static LanternSceneContext Current { get; private set; }
    public LightDoor LightDoor => _lightDoor;

    private new void Awake()
    {
        base.Awake();
        Current = this;
        _lightDoor = FindObjectOfType<LightDoor>();
        _isSilenced = true;

        foreach (var relation in _lanternRelations)
        {
            relation.ConnectionSparkEffect = Instantiate(_sparkEffectPrefab);
            relation.ConnectionSparkEffect.SetConnection(relation.A, relation.B);
            relation.ConnectionSparkEffect.StartTravel();
        }
        StartCoroutine(UnsilenceCoroutine());

    }
    private void Update()
    {
        if (_StopCheckingConnections) //TEMP..?
            return;
        foreach(var relation in _lanternRelations)
        {
            if (!relation.IsConnected)
            {
                if (relation.A.IsLightOn && relation.B.IsLightOn && CanRayBeReached(relation.A, relation.B))
                {
                    Connect(relation);
                    print("Connect " + relation.A.name + ", " + relation.B.name);
                }
            }
            else
            {
                if (!relation.A.IsLightOn || !relation.B.IsLightOn || !CanRayBeReached(relation.A, relation.B))
                {
                    Disconnect(relation);
                    print("Disconnect " + relation.A.name + ", " + relation.B.name);
                }
            }
        }
    }
    
    public void RecordActivationTime(LanternLike lantern)
    {
        _lanternActivationOrder.Remove(lantern);
        _lanternActivationOrder.Add(lantern);
    }
    IEnumerator UnsilenceCoroutine()
    {
        yield return new WaitForSeconds(_silenceTimeOnStart);
        _isSilenced = false;
    }

    public bool IsAllRelationsFullyConnected(params LanternLike [] exceptions)
    {
        foreach(var relation in _lanternRelations)
        {
            if (exceptions.Count(x => x == relation.A || x == relation.B) > 0)
                continue;
            if (!relation.IsConnectionDone)
                return false;
        }
        return true;
    }
    public HashSet<LanternLike> GetRelatedLanterns(LanternLike lantern)
    {
        HashSet<LanternLike> result = new HashSet<LanternLike>();
        foreach(var relation in _lanternRelations)
        {
            if (relation.A == lantern)
                result.Add(relation.B);
            else if (relation.B == lantern)
                result.Add(relation.A);
        }
        return result;
    }
    bool CanRayBeReached(LanternLike a, LanternLike b)
    {
        Vector2 rayDirection = b.LightPoint.position - a.LightPoint.position;
        var hits = new RaycastHit2D[MaxRayCastHitCount];
        Physics2D.RaycastNonAlloc(a.LightPoint.position, rayDirection, hits, MaxRayCastDistance, _beamObstacleLayers);
        foreach (var hit in hits)
        {
            if (hit.collider.transform == a.transform)
                continue;
            if (hit.collider.transform == b.transform)
                return true;
            else
                return false;
        }

        return false;
    }
    bool IsTurnedOnInOrder(LanternLike first, LanternLike second)
    {
        foreach(var lantern in _lanternActivationOrder)
        {
            if (lantern == first)
                return true;
            else if (lantern == second)
                return false;
        }
        return false;
    }
    void Connect(LanternRelation relation)
    {
        if (relation.IsConnected)
            return;
        if (relation.Beam == null)
        {
            relation.Beam = Instantiate<LightBeam>(_beamPrefab);
        }
        SetBeamConnections(relation);

        relation.ConnectionSparkEffect.gameObject.SetActive(false);

        if (relation.A.transform == _lightDoor.transform || relation.B.transform == _lightDoor.transform)
        {
            if (!_lightDoor.IsOpened)
                SceneEffectManager.Instance.PushCutscene(new Cutscene(this, LastConnectionCameraCoroutine(relation)));   
        }
        else
        {
            StartCoroutine(ConnectionCoroutine(relation));
        }

    }
    void SetBeamConnections(LanternRelation relation)
    {
        if (relation.A.transform == _lightDoor.transform)
            relation.Beam.SetLanterns(relation.B, relation.A);
        else if (relation.B.transform == _lightDoor.transform)
            relation.Beam.SetLanterns(relation.A, relation.B);
        else if (IsTurnedOnInOrder(relation.A, relation.B))
            relation.Beam.SetLanterns(relation.B, relation.A);
        else
            relation.Beam.SetLanterns(relation.A, relation.B);
    }
    IEnumerator LastConnectionCameraCoroutine(LanternRelation relation)
    {
        _StopCheckingConnections = true;

        //랜턴으로 카메라 이동 후 대기
        SceneEffectManager.Instance.Camera.StartFollow(relation.A.transform == _lightDoor ? relation.B.LightPoint : relation.A.LightPoint);
        InputManager.Instance.ChangeInputSetter(_lastConnectionInputSetter);
        yield return new WaitForSeconds(_cameraLastLanternStayDuration);

        //레이저 발사
        SceneEffectManager.Instance.Camera.StartConstantShake(_beamShootingPreset);
        StartCoroutine(ConnectionCoroutine(relation));
        _lastConnectionCameraPoint.position = relation.Beam.CurrentShootingPosition;
        SceneEffectManager.Instance.Camera.StartFollow(_lastConnectionCameraPoint);
        while (!relation.Beam.IsShootingDone)
        {
            _lastConnectionCameraPoint.position = relation.Beam.CurrentShootingPosition;
            yield return null;
        }
        _soundList.PlaySFX("SE_LightDoor_Contact");
        SceneEffectManager.Instance.Camera.StartShake(_beamHitDoorPreset);
        yield return new WaitForSeconds(_lastBeamDuration);
        SceneEffectManager.Instance.Camera.StopConstantShake();

        relation.Beam.gameObject.SetActive(false);
        //빔 사라진 후 문열기 시작
        yield return _lightDoor.OpenCoroutine();
        yield return new WaitForSeconds(_cameraDoorStayDuration);

        InputManager.Instance.ChangeToDefaultSetter();
    }
    IEnumerator ConnectionCoroutine(LanternRelation relation)
    {
        if(!_isSilenced)
            _soundList.PlaySFX("SE_Lantern_Line");
        relation.Beam.gameObject.SetActive(true);
        if (!_isSilenced)
            yield return new WaitUntil(()=>relation.Beam.IsShootingDone);
        relation.A.OnBeamConnected(relation.Beam);
        relation.B.OnBeamConnected(relation.Beam);
    }
    void Disconnect(LanternRelation relation)
    {
        if (!relation.IsConnected)
            return;
        relation.Beam.gameObject.SetActive(false);
        relation.ConnectionSparkEffect.gameObject.SetActive(true);
        relation.A.OnBeamDisconnected(relation.Beam);
        relation.B.OnBeamDisconnected(relation.Beam);
    }
    public void DisconnectFromAll(LanternLike lantern)
    {
        foreach (var relation in _lanternRelations)
        {
            if (relation.A == lantern || relation.B == lantern)
                Disconnect(relation);
        }
    }
    protected override Result SceneSpecificBuild()
    {
        Result buildResult = Result.Success;

        if (_lightDoor == null)
        {
            _lightDoor = FindObjectOfType<LightDoor>();
            if (_lightDoor == null)
            {
                Debug.LogWarning("Light Door is not set!");
                buildResult = Result.Fail;
            }
        }

        foreach(var relation in _lanternRelations)
        {

            if ((relation.A.transform == _lightDoor.transform || relation.B.transform == _lightDoor.transform) &&
                    _lightDoor.IsOpened)
                relation.ConnectionSparkEffect.gameObject.SetActive(false);
        }

        return buildResult;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        foreach(var relation in _lanternRelations)
        {
            if (relation.A == null || relation.B == null)
                continue;
            Gizmos.DrawLine(relation.A.LightPoint.position, relation.B.LightPoint.position);
        }
    }

}