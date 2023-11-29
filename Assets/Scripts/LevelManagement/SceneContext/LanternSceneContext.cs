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
        public bool IsConnected => Beam != null && Beam.gameObject.activeInHierarchy;
        public bool IsConnectionDone => IsConnected && Beam.IsShootingDone;
    }
    public new static LanternSceneContext Current { get; private set; }

    [SerializeField] LightBeam _beamPrefab;
    [SerializeField] LayerMask _beamObstacleLayers;
    [SerializeField] LightDoor _lightDoor;
    [SerializeField] List<LanternRelation> _lanternRelations;

    List<LanternLike> _lanternActivationOrder = new List<LanternLike>();

    const float MaxRayCastDistance = 1000f;
    const uint MaxRayCastHitCount = 3;
    public void RecordActivationTime(LanternLike lantern)
    {
        _lanternActivationOrder.Remove(lantern);
        _lanternActivationOrder.Add(lantern);
    }
    private new void Awake()
    {
        base.Awake();
        Current = this;
        if (_lightDoor == null)
            Debug.LogWarning("Light Door is not set!");
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
    private void Update()
    {
        foreach(var relation in _lanternRelations)
        {
            if (!relation.IsConnected)
            {
                if (relation.A.IsLightOn && relation.B.IsLightOn && CanRayBeReached(relation.A, relation.B))
                    Connect(relation);
            }
            else
            {
                if (!relation.A.IsLightOn || !relation.B.IsLightOn || !CanRayBeReached(relation.A, relation.B))
                    Disconnect(relation);
            }
        }
    }
    bool CanRayBeReached(LanternLike a, LanternLike b)
    {
        Vector2 rayDirection = b.transform.position - a.transform.position;
        var hits = new RaycastHit2D[MaxRayCastHitCount];
        Physics2D.RaycastNonAlloc(a.transform.position, rayDirection, hits, MaxRayCastDistance, _beamObstacleLayers);
        foreach (var hit in hits)
        {
            if (hit.transform == a.transform)
                continue;
            if (hit.transform == b.transform)
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


        StartCoroutine(ConnectionCoroutine(relation));
    }
    IEnumerator ConnectionCoroutine(LanternRelation relation)
    {
        if (relation.A.transform == _lightDoor.transform)
            relation.Beam.SetLanterns(relation.B, relation.A);
        else if (relation.B.transform == _lightDoor.transform)
            relation.Beam.SetLanterns(relation.A, relation.B);
        else if (IsTurnedOnInOrder(relation.A, relation.B))
            relation.Beam.SetLanterns(relation.B, relation.A);
        else
            relation.Beam.SetLanterns(relation.A, relation.B);
        relation.Beam.gameObject.SetActive(true);
        yield return new WaitUntil(()=>relation.Beam.IsShootingDone);
        relation.A.OnBeamConnected(relation.Beam);
        relation.B.OnBeamConnected(relation.Beam);
    }
    void Disconnect(LanternRelation relation)
    {
        if (!relation.IsConnected)
            return;
        relation.Beam.gameObject.SetActive(false);
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



        return buildResult;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        foreach(var relation in _lanternRelations)
        {
            if (relation.A == null || relation.B == null)
                continue;
            Gizmos.DrawLine(relation.A.transform.position, relation.B.transform.position);
        }
    }

}