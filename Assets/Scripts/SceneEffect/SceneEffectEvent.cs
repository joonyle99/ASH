
using System.Collections.Generic;
using UnityEngine;

public class SceneEventComparator : IComparer<SceneEffectEvent>
{
    public int Compare(SceneEffectEvent first, SceneEffectEvent second)
    {
        if (first.Priority <= second.Priority)
            return -1;
        return 1;
    }
}
[System.Serializable]
public class SceneEffectEvent
{
    public bool Enabled
    {
        get {  return _enabled; }
        set
        {
            if (value != _enabled)
            {
                if (value)
                    OnEnter();
                else
                    OnExit();
                _enabled = value;
            }
        }
    }
    bool _enabled = false;
    public enum MergePolicy
    {
        OverrideWithRecent, PlayTogether
    }
    public enum EventPriority
    {
        MovingObjects, MajorEvent
    }
    public EventPriority Priority { get; protected set; }
    public MergePolicy  MergePolicyWithSamePriority { get; protected set; }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}

namespace SceneEvents
{
    [System.Serializable]
    public class FollowObjects : SceneEffectEvent
    {
        Transform[] _targets;
        public FollowObjects(EventPriority priority, MergePolicy mergePolicyWithSamePriority, params Transform[] targets)
        {
            Priority = priority;
            _targets = targets;
            MergePolicyWithSamePriority = mergePolicyWithSamePriority;
        }
        public override void OnEnter()
        {
            SceneEffectManager.Instance.Camera.AddFollowTargets(_targets);
        }
        public override void OnExit()
        {
            SceneEffectManager.Instance.Camera.RemoveFollowTargets(_targets);
        }
    }
}