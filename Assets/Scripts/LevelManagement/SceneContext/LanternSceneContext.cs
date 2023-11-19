using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public sealed class LanternSceneContext : SceneContext
{
    [System.Serializable]
    class LanternRelation
    {
        public Lantern A;
        public Lantern B;
        [HideInInspector] public LightBeam Beam;
        public bool IsConnected => Beam != null && Beam.gameObject.activeInHierarchy;
    }
    public new static LanternSceneContext Current { get; private set; }

    [SerializeField] LightBeam _beamPrefab;
    [SerializeField] LayerMask _beamObstacleLayers;
    [SerializeField] List<LanternRelation> _lanternRelations;

    List<Lantern> _lanternActivationOrder = new List<Lantern>();
    public void RecordActivationTime(Lantern lantern)
    {
        _lanternActivationOrder.Remove(lantern);
        _lanternActivationOrder.Add(lantern);
    }
    private new void Awake()
    {
        base.Awake();
        Current = this;
    }
    public HashSet<Lantern> GetRelatedLanterns(Lantern lantern)
    {
        HashSet<Lantern> result = new HashSet<Lantern>();
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
    bool CanRayBeReached(Lantern a, Lantern b)
    {
        Vector2 rayDirection = b.transform.position - a.transform.position;
        float distance = rayDirection.magnitude + 1;
        var hits = Physics2D.RaycastAll(a.transform.position, rayDirection, distance, _beamObstacleLayers);
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
    bool IsTurnedOnInOrder(Lantern first, Lantern second)
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

        if (IsTurnedOnInOrder(relation.A, relation.B))
            relation.Beam.SetLanterns(relation.A, relation.B);
        else
            relation.Beam.SetLanterns(relation.B, relation.A);

        relation.Beam.gameObject.SetActive(true);
    }
    void Disconnect(LanternRelation relation)
    {
        if (!relation.IsConnected)
            return;
        relation.Beam.gameObject.SetActive(false);
    }
    public void DisconnectFromAll(Lantern lantern)
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



}